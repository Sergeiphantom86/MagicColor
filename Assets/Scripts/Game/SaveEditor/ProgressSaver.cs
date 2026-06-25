using System;
using PuzzleEditor.RouletteEditor;
using PuzzleEditor.SoundEditor;
using YG;

namespace Game.SaveEditor
{
    public class ProgressSaver : IProgressSaver
    {
        public SavesYG Saves
        {
            get
            {
                if (YG2.saves == null)
                {
                    return null;
                }
                return YG2.saves;
            }
        }

        public void StartInitYG2() => YG2.StartInit();

        public bool IdentifyDevice()
        {
            return YG2.envir.isMobile;
        }

        public void SetCountQuest(int count) =>
            WithSaves(savesYG2 => savesYG2.SetCountQuest(count));

        public void SetMusicTime(float time) => WithSaves(savesYG2 => savesYG2.SetMusicTime(time));

        public bool TryEnableFollowingQuest(int index) =>
            WithSaves(savesYG2 => savesYG2.TryEnableFollowingQuest(index), false);

        public void SaveSpinsCount(int spins) =>
            WithSaves(savesYG2 => savesYG2.SaveSpinsCount(spins));

        public void SetQuantityAbilities(int spins) =>
            WithSaves(savesYG2 => savesYG2.SetQuantityAbilities(spins));

        public void SetTutorial(int index) => WithSaves(savesYG2 => savesYG2.SetTutorial(index));

        public void SetTutorialBasics() => WithSaves(savesYG2 => savesYG2.SetTutorialBasics());

        public void SetUnblockingTutorial() =>
            WithSaves(savesYG2 => savesYG2.SetUnblockingTutorial());

        public void SetAbilityTutorial() => WithSaves(savesYG2 => savesYG2.SetAbilityTutorial());

        public void DisableTutorialMenu() => WithSaves(savesYG2 => savesYG2.DisableTutorialMenu());

        public void SaveBalanceAfterPurchase(long balans) =>
            WithSaves(savesYG2 => savesYG2.SaveBalanceAfterPurchase(balans));

        public void SetReward(int reward) => WithSaves(savesYG2 => savesYG2.SetReward(reward));

        public void MakeTransparent(bool transparent) =>
            WithSaves(savesYG2 => savesYG2.MakeTransparent(transparent));

        public void SetCurrency(Currency currency, long balance) =>
            WithSaves(savesYG2 => savesYG2.SetCurrency(currency, balance));

        public void SetQuestIndex(int index) =>
            WithSaves(savesYG2 => savesYG2.SetQuestIndex(index));

        public void SetMaxReachedQuestIndex() =>
            WithSaves(savesYG2 => savesYG2.SetMaxReachedQuestIndex());

        public void SetCountStars(int count) =>
            WithSaves(savesYG2 => savesYG2.SetCountStars(count));

        public void SetVolume(VolumeChanger changer, float volume) =>
            WithSaves(savesYG2 => savesYG2.SetVolume(changer, volume));

        public void ObstacleSwitch(bool isOn) =>
            WithSaves(savesYG2 => savesYG2.ObstacleSwitch(isOn));

        public void SetCurrentLanguage(string langCode) =>
            WithSaves(savesYG2 => savesYG2.SetCurrentLanguage(langCode));

        public void SaveProgress() => YG2.SaveProgress();

        public void SetDefaultValues() => YG2.SetDefaultSaves();

        public void SetLeaderboard(string name, int score) => YG2.SetLeaderboard(name, score);

        public string GetTranslationLanguage() => YG2.lang;

        public void SwitchLanguage(string langCode) => YG2.SwitchLanguage(langCode);

        public void RewardedAdvShow(string rewardID, Action action = null) =>
            YG2.RewardedAdvShow(rewardID, action);

        public void SubscribeADSReward(
            Action<string> onReward,
            Action onOpen,
            Action onClose,
            Action onError
        ) => Subscribe(onReward, onOpen, onClose, onError);

        public void UnsubscribeADSReward(
            Action<string> onReward,
            Action onOpen,
            Action onClose,
            Action onError
        ) => Unsubscribe(onReward, onOpen, onClose, onError);

        public bool CanShowAd()
        {
            return YG2.nowRewardAdv == false && YG2.nowAdsShow == false;
        }

        public void SubscribeSDKData(Action onLoaded)
        {
            if (onLoaded != null)
                YG2.onGetSDKData += onLoaded;
        }

        public void UnsubscribeSDKData(Action onLoaded)
        {
            if (onLoaded != null)
                YG2.onGetSDKData -= onLoaded;
        }

        public void SubscribeSwitchLang(Action<string> onLangChanged)
        {
            if (onLangChanged != null)
                YG2.onSwitchLang += onLangChanged;
        }

        public void UnsubscribeSwitchLang(Action<string> onLangChanged)
        {
            if (onLangChanged != null)
                YG2.onSwitchLang -= onLangChanged;
        }

        private void WithSaves(Action<SavesYG> action)
        {
            if (Saves == null)
                return;

            action.Invoke(Saves);
        }

        private T WithSaves<T>(Func<SavesYG, T> func, T defaultValue = default)
        {
            return Saves == null ? defaultValue : func.Invoke(Saves);
        }

        private void Subscribe(
            Action<string> onReward,
            Action onOpen,
            Action onClose,
            Action onError
        )
        {
            if (onReward != null)
                YG2.onRewardAdv += onReward;

            if (onOpen != null)
                YG2.onOpenRewardedAdv += onOpen;

            if (onClose != null)
                YG2.onCloseRewardedAdv += onClose;

            if (onError != null)
                YG2.onErrorRewardedAdv += onError;
        }

        private void Unsubscribe(
            Action<string> onReward,
            Action onOpen,
            Action onClose,
            Action onError
        )
        {
            if (onReward != null)
                YG2.onRewardAdv -= onReward;

            if (onOpen != null)
                YG2.onOpenRewardedAdv -= onOpen;

            if (onClose != null)
                YG2.onCloseRewardedAdv -= onClose;

            if (onError != null)
                YG2.onErrorRewardedAdv -= onError;
        }
    }
}