namespace UI
{
    public interface IUiService
    {
        UIView uiView { get; }
        UiService.Events events { get; }

        public T GetWindow<T>() where T : BaseWindow;
        public void PreloadWindow<T>() where T : BaseWindow;
        public T ShowWindow<T>(bool insert = false, bool forceShow = false) where T : BaseWindow;
        public void ShowWindowOver<T>() where T : BaseWindow;
        public void ClearAllWindows();
        
        bool HasOpenGuiWindow();
    }
}   