namespace Sources.cdreyer.SaveSystem
{
    public interface ISavable<T>
    {
        string FileName { get; }
        void Save();

        T Load();
        T GetBase();
    }
}