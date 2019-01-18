using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using PM.UsefulThings;
using UnityEngine.Audio;
using PM.UsefulThings.Extensions;

namespace PM.UsefulThings
{
	public class SoundManager : PrefabMonoSingleton<SoundManager>
	{
		public const int maxNumberOfBackupSources = 10;
		public const int maxNumberOfBackupLoopSources = 10;

		public AudioMixer MainMixer;
		public AudioSource MusicSource;
		public AudioSource VoiceSource;
		public AudioSource SingleSfxSource;
		public AudioSource LoopSfxSource;
		[SerializeField]
		public Queue<AudioSource> BackupSources = new Queue<AudioSource>();
		[SerializeField]
		public Queue<AudioSource> BackupLoopSources = new Queue<AudioSource>();

		[Range(0, 1)]
		public float _masterVolume = 0.3f;
		[Range(0, 1)]
		public float _musicVolume = 1;
		[Range(0, 1)]
		public float _voiceVolume = 1;
		[Range(0, 1)]
		public float _sfxVolume = 1;


		[Space]
		public AudioClip[] MenuMusic;
		public AudioClip[] GameplayMusic;
		[Space]
		public AudioClip GameOverVoice;
		[Space]
		public AudioClip[] Blasters;


		public float MasterVolume
		{
			get
			{
				return PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : _masterVolume;
			}
			set
			{
				if (_masterVolume != value)
				{
					_masterVolume = value;
					PlayerPrefs.SetFloat("MasterVolume", value);
					UpdateVolume();
				}
			}
		}

		public float MusicVolume
		{
			get
			{
				return PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : _musicVolume;
			}
			set
			{
				if (_masterVolume != value)
				{
					_musicVolume = value;
					PlayerPrefs.SetFloat("MusicVolume", value);
					UpdateVolume();
				}
			}
		}

		public float VoiceVolume
		{
			get
			{
				return PlayerPrefs.HasKey("VoiceVolume") ? PlayerPrefs.GetFloat("VoiceVolume") : _voiceVolume;
			}
			set
			{
				if (_masterVolume != value)
				{
					_voiceVolume = value;
					PlayerPrefs.SetFloat("VoiceVolume", value);
					UpdateVolume();
				}
			}
		}

		public float SfxVolume
		{
			get
			{
				return PlayerPrefs.HasKey("SfxVolume") ? PlayerPrefs.GetFloat("SfxVolume") : _sfxVolume;
			}
			set
			{
				if (_masterVolume != value)
				{
					_sfxVolume = value;
					PlayerPrefs.SetFloat("SfxVolume", value);
					UpdateVolume();
				}
			}
		}

		private bool _isMasterOn;
		public bool IsMasterOn
		{
			get
			{
				return _isMasterOn;
			}
			set
			{
				if (_isMasterOn == value)
				{
					return;
				}

				_isMasterOn = value;
				PlayerPrefs.SetInt("Master", value ? 1 : 0);

				// music
				// if set "on", continue
				if (value)
				{
					if (!MusicSource.isPlaying && IsMusicOn)
					{
						MusicSource.UnPause();
					}
				}
				// else you should stop
				else
				{
					if (MusicSource.isPlaying)
					{
						MusicSource.Pause();
					}
				}

				// sfx
				if (!value)
				{
					//you should stop
					if (SingleSfxSource.isPlaying)
						SingleSfxSource.Stop();
					if (LoopSfxSource.isPlaying)
						LoopSfxSource.Stop();
					foreach (var source in BackupSources)
					{
						source.Stop();
					}
					foreach (var source in BackupLoopSources)
					{
						source.Stop();
					}
				}

				// voice
				if (!value)
				{
					// you should stop
					if (VoiceSource.isPlaying)
					{
						VoiceSource.Stop();
					}
				}
			}
		}

		private bool _isSfxOn;
		public bool IsSfxOn
		{
			get
			{
				return _isSfxOn && IsMasterOn;
			}
			set
			{
				if (_isSfxOn == value)
				{
					return;
				}

				_isSfxOn = value;
				PlayerPrefs.SetInt("Sfx", value ? 1 : 0);

				//if set "on", you don't need to play sfx again
				if (value)
					return;

				//else you should stop
				if (SingleSfxSource.isPlaying)
					SingleSfxSource.Stop();
				if (LoopSfxSource.isPlaying)
					LoopSfxSource.Stop();
				foreach (var source in BackupSources)
				{
					source.Stop();
				}
				foreach (var source in BackupLoopSources)
				{
					source.Stop();
				}
			}
		}
		private bool _isMusicOn;
		public bool IsMusicOn
		{
			get
			{
				return _isMusicOn && IsMasterOn;
			}
			set
			{
				if (_isMusicOn == value)
				{
					return;
				}

				_isMusicOn = value;
				PlayerPrefs.SetInt("Music", value ? 1 : 0);

				//if set "on", continue
				if (value)
				{
					if (!MusicSource.isPlaying && IsMasterOn)
					{
						MusicSource.UnPause();
					}
				}
				//else you should stop
				else
				{
					if (MusicSource.isPlaying)
					{
						MusicSource.Pause();
					}
				}
			}
		}
		private bool _isVoiceOn;
		public bool IsVoiceOn
		{
			get
			{
				return _isVoiceOn && IsMasterOn;
			}
			set
			{
				if (_isVoiceOn == value)
				{
					return;
				}

				_isVoiceOn = value;
				PlayerPrefs.SetInt("Voice", value ? 1 : 0);

				//if set "on", continue
				if (value)
				{
					if (!VoiceSource.isPlaying)
					{
						VoiceSource.UnPause();
					}
				}
				//else you should stop
				else
				{
					if (VoiceSource.isPlaying)
					{
						VoiceSource.Pause();
					}
				}
			}
		}

		private bool isMusicShuffle { get; set; }
		private AudioClip[] musicCollection;
		private int increment;
		private Dictionary<AudioSource, int> notebook = new Dictionary<AudioSource, int>();

		private void Start()
		{
			_isMasterOn = PlayerPrefs.HasKey("Master") ? PlayerPrefs.GetInt("Master") == 1 : true;
			_isMusicOn = PlayerPrefs.HasKey("Misuc") ? PlayerPrefs.GetInt("Misic") == 1 : true;
			_isVoiceOn = PlayerPrefs.HasKey("Voice") ? PlayerPrefs.GetInt("Voice") == 1 : true;
			_isSfxOn = PlayerPrefs.HasKey("Sfx") ? PlayerPrefs.GetInt("Sfx") == 1 : true;

			if (MusicSource == null)
			{
				MusicSource = this.gameObject.AddComponent<AudioSource>();
			}
			if (VoiceSource == null)
			{
				VoiceSource = this.gameObject.AddComponent<AudioSource>();
			}
			if (SingleSfxSource == null)
			{
				SingleSfxSource = this.gameObject.AddComponent<AudioSource>();
			}
			if (LoopSfxSource == null)
			{
				LoopSfxSource = this.gameObject.AddComponent<AudioSource>();
			}

			HashSet<AudioSource> sources = new HashSet<AudioSource>();
			sources.Add(MusicSource);
			sources.Add(VoiceSource);
			sources.Add(SingleSfxSource);
			sources.Add(LoopSfxSource);
			if (sources.Count != 4)
				Debug.LogError("You put 1 source to 2 positions!");

			UpdateVolume();
		}

		private void Update()
		{
			//this is a loop realization for music
			if (IsMusicOn && !MusicSource.isPlaying && musicCollection != null && musicCollection.Length != 0)
			{
				AudioClip clip = null;

				//doesn't matter shuffle or not
				if (musicCollection.Length == 1)
				{
					clip = musicCollection[0];
				}
				//shuffle
				else if (isMusicShuffle)
				{
					do
					{
						clip = musicCollection[UnityEngine.Random.Range(0, musicCollection.Length)];
					} while (MusicSource.clip == clip);
				}
				//order
				else
				{
					clip = musicCollection[(musicCollection.IndexOf(MusicSource.clip) + 1) % musicCollection.Length];
				}

				PlayClip(MusicSource, clip);
			}

#if UNITY_EDITOR
			MasterVolume = _masterVolume;
			MusicVolume = _musicVolume;
			VoiceVolume = _voiceVolume;
			SfxVolume = _sfxVolume;
#endif
		}

		private void UpdateVolume()
		{
			MainMixer.SetFloat("Master", MasterVolume == 0 ? -80 : 20 * Mathf.Log10(MasterVolume));
			MainMixer.SetFloat("Music", MusicVolume == 0 ? -80 : 20 * Mathf.Log10(MusicVolume));
			MainMixer.SetFloat("Voice", VoiceVolume == 0 ? -80 : 20 * Mathf.Log10(VoiceVolume));
			MainMixer.SetFloat("Sfx", SfxVolume == 0 ? -80 : 20 * Mathf.Log10(SfxVolume));
		}



		public void ChangeMasterState()
		{
			IsMasterOn = !IsMasterOn;
		}

		public void ChangeMusicState()
		{
			IsMusicOn = !IsMusicOn;
		}

		public void ChangeVoiceState()
		{
			IsVoiceOn = !IsVoiceOn;
		}

		public void ChangeSfxState()
		{
			IsSfxOn = !IsSfxOn;
		}


		public KeyValuePair<AudioSource, int> PlaySfx(AudioClip[] clip, float volume = 1, float pitch = 1, bool isPlayInLoop = false, bool isParallel = false)
		{
			if (clip.Length == 0)
				throw new System.Exception("Missing SFX");

			var randomClip = clip[UnityEngine.Random.Range(0, clip.Length)];

			return PlaySfx(randomClip, volume, pitch, isPlayInLoop, isParallel);
		}

		/// <summary>
		/// plays clip by parameters, returns pair, that you can use to stop that clip safely. If on that source will start another clip, pair will become invalid.
		/// if error, return will contain pair with key == null
		/// </summary>
		/// <param name="clip"></param>
		/// <param name="isPlayInLoop"></param>
		/// <param name="isParallel"></param>
		/// <param name="pitch"></param>
		public KeyValuePair<AudioSource, int> PlaySfx(AudioClip clip, float volume = 1, float pitch = 1, bool isPlayInLoop = false, bool isParallel = false)
		{
			if (!IsSfxOn)
				return new KeyValuePair<AudioSource, int>(null, 0);

			if (clip == null)
				return new KeyValuePair<AudioSource, int>(null, 0);

			if (isParallel)
			{
				var sources = isPlayInLoop ? BackupLoopSources : BackupSources;
				var max = isPlayInLoop ? maxNumberOfBackupLoopSources : maxNumberOfBackupSources;


				if (sources.Count > 0)
				{
					if (sources.Peek().isPlaying)
					{
						//there is still hope we have enough sources, so try find some in the middle
						foreach (var source in sources)
						{
							if (!source.isPlaying)
							{
								return PlayClip(source, clip, isPlayInLoop, pitch, volume);
							}
						}
					}
					else
					{
						var source = sources.Dequeue();
						sources.Enqueue(source);
						return PlayClip(source, clip, isPlayInLoop, pitch, volume);
					}
				}

				//if we're here, it means there is no free backup source
				if (sources.Count < max)
				{
					//create one
					var source = this.gameObject.AddComponent<AudioSource>();
					source.outputAudioMixerGroup = SingleSfxSource.outputAudioMixerGroup;
					sources.Enqueue(source);
					return PlayClip(source, clip, isPlayInLoop, pitch, volume);
				}
				else
				{
					//we can't create more sources
					Debug.LogWarning("there is no free audio source to play clip " + clip.name);

					//I think there is a best chance to keep it hidden from user is play it anyway.
					var source = sources.Dequeue();
					sources.Enqueue(source);
					return PlayClip(source, clip, isPlayInLoop, pitch, volume);
				}
			}
			else
			{
				if (isPlayInLoop)
					return PlayClip(LoopSfxSource, clip, isPlayInLoop, pitch, volume);
				else
					return PlayClip(SingleSfxSource, clip, isPlayInLoop, pitch, volume);
			}
		}

		/// <summary>
		/// plays clip by parameters, returns pair, that you can use to stop that clip safely. If on that source will start another clip, pair will become invalid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="clip"></param>
		/// <param name="isPlayInLoop"></param>
		/// <param name="pitch"></param>
		/// <returns></returns>
		private void PlaySingleSfx(AudioSource source, AudioClip clip, bool isPlayInLoop = false, float pitch = 1, float volume = 1)
		{
			if (!IsSfxOn)
				return;

			if (clip == null)
				return;

			source.volume = volume;
			source.loop = isPlayInLoop;
			source.pitch = pitch;
			source.PlayOneShot(clip);
		}

		public void PlayMusic(AudioClip clip)
		{
			PlayMusic((new AudioClip[] { clip }));
		}

		/// <summary>
		/// if shuffle is off, music will play by order
		/// </summary>
		/// <param name="clips"></param>
		/// <param name="_isMusicShuffle"></param>
		public void PlayMusic(AudioClip[] clips, bool _isMusicShuffle = false)
		{
			if (clips.Length == 0)
			{
				return;
			}

			this.musicCollection = clips;
			this.isMusicShuffle = _isMusicShuffle;

			//set, but not play if you shouldn't
			if (!IsMusicOn)
				return;

			if (this.isMusicShuffle)
				PlayClip(MusicSource, musicCollection[UnityEngine.Random.Range(0, musicCollection.Length)], volume: MusicSource.volume);
			else
				PlayClip(MusicSource, musicCollection[0], volume: MusicSource.volume);
		}

		/// <summary>
		/// voice is simply 1 channel clip
		/// </summary>
		/// <param name="clips"></param>
		/// <param name="_isMusicShuffle"></param>
		public void PlayVoice(AudioClip clip, float volume = 1, float pitch = 1)
		{
			//do not play if you shouldn't
			if (!IsVoiceOn)
				return;

			PlayClip(VoiceSource, clip, false, pitch, MasterVolume * VoiceVolume * volume);
		}

		/// <summary>
		/// plays clip by parameters, returns pair, that you can use to stop that clip safely. If on that source will start another clip, pair will become invalid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="clip"></param>
		/// <param name="isPlayInLoop"></param>
		/// <param name="pitch"></param>
		/// <returns></returns>
		private KeyValuePair<AudioSource, int> PlayClip(AudioSource source, AudioClip clip, bool isPlayInLoop = false, float pitch = 1, float volume = 1)
		{
			source.volume = volume;
			source.clip = clip;
			source.loop = isPlayInLoop;
			source.pitch = pitch;
			source.Play();
			var pairValue = getIncrement();
			NotePair(source, pairValue);
			return new KeyValuePair<AudioSource, int>(source, pairValue);
		}

		public bool IsSfxPlaying(AudioClip clip)
		{
			if (SingleSfxSource.isPlaying && SingleSfxSource.clip == clip)
				return true;

			if (LoopSfxSource.isPlaying && LoopSfxSource.clip == clip)
				return true;

			foreach (var source in BackupSources)
			{
				if (source.isPlaying && source.clip == clip)
					return true;
			}

			foreach (var source in BackupLoopSources)
			{
				if (source.isPlaying && source.clip == clip)
					return true;
			}

			return false;
		}

		public bool IsVoicePlaying(AudioClip clip)
		{
			if (VoiceSource.isPlaying && VoiceSource.clip == clip)
				return true;

			return false;
		}

		public bool IsMusicClipPlayingNow(AudioClip clip)
		{
			return MusicSource.clip == clip;
		}

		public bool IsMusicClipInQueue(AudioClip clip)
		{
			return musicCollection.Contains(clip);
		}

		public bool IsMusicCollectionIsPlaying(AudioClip[] clips)
		{
			if (musicCollection == clips)
				return true;

			if (musicCollection.Length != clips.Length)
				return false;

			int match = 0;
			foreach (var clip in clips)
			{
				if (musicCollection.Contains(clip))
				{
					match++;
				}
			}
			return match == clips.Length;
		}

		/// <summary>
		/// stops every audiosource, that plays clip. if music is on, music source will start playing again next clip of collection in next update
		/// </summary>
		/// <param name="clip"></param>
		public void StopClip(AudioClip clip)
		{
			if (MusicSource.isPlaying && MusicSource.clip == clip)
				MusicSource.Stop();

			if (VoiceSource.isPlaying && VoiceSource.clip == clip)
				VoiceSource.Stop();

			if (SingleSfxSource.isPlaying && SingleSfxSource.clip == clip)
				SingleSfxSource.Stop();

			if (LoopSfxSource.isPlaying && LoopSfxSource.clip == clip)
				LoopSfxSource.Stop();

			foreach (var source in BackupSources)
			{
				if (source.isPlaying && source.clip == clip)
					source.Stop();
			}

			foreach (var source in BackupLoopSources)
			{
				if (source.isPlaying && source.clip == clip)
					source.Stop();
			}
		}

		/// <summary>
		/// stops the specific source. if music is on, music source will start playing again next clip of collection in next update
		/// </summary>
		/// <param name="source"></param>
		public void StopAudioSource(AudioSource source)
		{
			source.Stop();
		}

		/// <summary>
		/// stops the specific source by pair if it's still valid. if music is on, music source will start playing again next clip of collection in next update
		/// </summary>
		/// <param name="pair"></param>
		public void StopByPair(KeyValuePair<AudioSource, int> pair)
		{
			if (IsPairValid(pair))
			{
				pair.Key.Stop();
			}
		}


		private void NotePair(AudioSource key, int value)
		{
			//key can't be null
			if (key == null)
				return;

			if (notebook.ContainsKey(key))
			{
				notebook[key] = value;
			}
			else
			{
				notebook.Add(key, value);
			}
		}

		private bool IsPairValid(KeyValuePair<AudioSource, int> pair)
		{
			return pair.Key != null && notebook.ContainsKey(pair.Key) && notebook[pair.Key] == pair.Value;
		}

		private int getIncrement()
		{
			return increment++;
		}
	}
}