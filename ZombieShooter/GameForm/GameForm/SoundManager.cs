using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

public class SoundManager
{
    private Dictionary<string, byte[]> soundResources;  
    private List<WaveOutEvent> activeSounds;  

    public SoundManager()
    {
        soundResources = new Dictionary<string, byte[]>();
        activeSounds = new List<WaveOutEvent>();
    }

    public void LoadSound(string name, Stream resourceStream)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            resourceStream.CopyTo(ms);
            soundResources[name] = ms.ToArray();
        }
    }

    public void PlaySound(string name)
    {
        if (soundResources.ContainsKey(name))
        {
            Task.Run(() =>
            {
                var soundStream = new MemoryStream(soundResources[name]);
                var waveProvider = new WaveFileReader(soundStream);
                var waveOut = new WaveOutEvent();

                waveOut.Init(waveProvider);
                lock (activeSounds)
                {
                    activeSounds.Add(waveOut); 
                }

                waveOut.Play();

                // Cleanup after playback completes
                waveOut.PlaybackStopped += (sender, args) =>
                {
                    waveOut.Dispose();
                    waveProvider.Dispose();
                    soundStream.Dispose();
                    lock (activeSounds)
                    {
                        activeSounds.Remove(waveOut);  
                    }
                };
            });
        }
    }
}
