namespace Game.SaveEditor
{
    public interface ITutorialProgress
    {
        public void SetTutorial(int index);

        public void DisableTutorialMenu();

        public void SetTutorialBasics();

        public void SetUnblockingTutorial();

        public void SetAbilityTutorial();
    }
}