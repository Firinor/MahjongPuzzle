using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
   public static SoundManager Instance;
   [SerializeField]
   private AudioConfig config;
   [SerializeField]
   private List<AudioSource> audioPool;
   
   private void Awake()
   {
      Instance = this;
   }
   
   public void PlayButtonClick(Vector3 position = default)
   {
      Play(position, config.ButtonClick, isPriority: true);
   }
   public void PlayTileStartCollide(Vector3 position = default)
   {
      Play(position, config.StartCollide, isPriority: true);
   }
   public void PlayTileEndCollide(Vector3 position = default)
   {
      Play(position, config.EndCollide, isPriority: true);
   }
   public void PlayTileSelect(Vector3 position = default)
   {
      Play(position, config.TileSelect);
   }
   public void PlayTileError(Vector3 position = default)
   {
      Play(position, config.TileError);
   }
   
   public void Play(Vector3 position, ClipSettings clipData, bool isPriority = false)
   {
      AudioSource source = audioPool.FirstOrDefault(a => !a.gameObject.activeSelf);

      if (source is null)
      {
         if(!isPriority)
            return;
         source = audioPool[0];
      }
      
      source.gameObject.SetActive(true);
      source.transform.position = position;
      source.pitch = 1 + Random.Range(-0.05f, 0.05f);
      source.volume = clipData.Volume;
      
      source.PlayOneShot(clipData.Clip);

      StartCoroutine(DisableAudioSource(source));
   }

   private IEnumerator DisableAudioSource(AudioSource source)
   {
      while (source.isPlaying)
      {
         yield return null;
      }
      
      source.gameObject.SetActive(false);
   }
}
