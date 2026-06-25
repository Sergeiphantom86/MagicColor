namespace Menu
{
    public interface ICarousel
    {
        int CurrentIndex { get; }

        float ScrollDuration { get; }

        void ScrollToButton(int index);

        void ShowRelative(int direction);
    }
}