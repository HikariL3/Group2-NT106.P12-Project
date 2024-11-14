using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

public class SoundManager
{
    private Dictionary<string, byte[]> soundResources;  // Store byte arrays of sounds
    private List<WaveOutEvent> activeSounds;  // Keep track of active sounds for concurrent playback

    public SoundManager()
    {
        soundResources = new Dictionary<string, byte[]>();
        activeSounds = new List<WaveOutEvent>();
    }

    // Load sound from Resources (as byte array)
    public void LoadSound(string name, Stream resourceStream)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            resourceStream.CopyTo(ms);
            soundResources[name] = ms.ToArray();
        }
    }

    // Play sound by name, independently, without interrupting other sounds
    public void PlaySound(string name)
    {
        if (soundResources.ContainsKey(name))
        {
            Task.Run(() =>
            {
                // Create new instances for each playback
                var soundStream = new MemoryStream(soundResources[name]);
                var waveProvider = new WaveFileReader(soundStream);
                var waveOut = new WaveOutEvent();

                waveOut.Init(waveProvider);
                lock (activeSounds)
                {
                    activeSounds.Add(waveOut);  // Add to list of active sounds
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
                        activeSounds.Remove(waveOut);  // Remove from active sounds
                    }
                };
            });
        }
    }
}
