using UnityEngine;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        private Sprite _sprite;

        public SavesYG()
        {
            MusicVolume = 0.8f;
            SoundVolume = 0.8f;
            CountStars = 0;
            IsMusicEnabled = true;
            NpcStates = new Dictionary<string, int>();
            PuzzleBestTimes = new Dictionary<string, float>();
        }

        public Dictionary<string, int> NpcStates { get; private set; }
        public Dictionary<string, float> PuzzleBestTimes { get; private set; }

        public float MusicVolume { get; private set; }
        public float SoundVolume {get; private set; }
        public bool IsMusicEnabled{get; private set;}
        public int CountStars{get; private set;}

        public int QuestID { get; private set; }

        public Sprite Sprite => _sprite;

        internal void SetSprite(Sprite sprite)
        {
            if (sprite == null) return;
            if (_sprite != null) return;

            _sprite = sprite;
        }

        public void ResetSprite()
        {
            _sprite = null;
        }

        public void SetVolume(string name, float volume)
        {
            if (nameof(MusicVolume) == name)
            {
                MusicVolume = volume;
            }
            else
            {
                SoundVolume = volume;
            }
        }

        public void SetQuestID(int questID)
        {
            //if(questID < 0 ) return;

            //Debug.Log("ijvbfov");
            
            QuestID = questID;
        }

        public void SetCountStars(int count)
        {
            CountStars = count;
        }
    }
}