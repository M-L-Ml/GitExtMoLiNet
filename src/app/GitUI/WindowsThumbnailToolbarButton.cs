using Microsoft.WindowsAPICodePack.Taskbar;

namespace GitUI
{

    /// <summary>
    /// This looks as a ViewModel of a button in the thumbnail toolbar
    /// </summary>
    public sealed class WindowsThumbnailToolbarButton
    {
        private bool _enabled = true;

        public WindowsThumbnailToolbarButton(string text, Image image, EventHandler<ThumbnailButtonClickedEventArgs> click)
        {
            Text = text;
            Image = image;
            Click = click;
        }

        public EventHandler<ThumbnailButtonClickedEventArgs> Click { get; }
        public Image Image { get; }
        public string Text { get; }
        public bool Enabled { get => _enabled && Click is not null; init => _enabled = value; }
    }
}
