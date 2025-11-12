public interface ISaveSystem
{
    void SaveGame(object saveData);
    T LoadGame<T>() where T : class;
    bool HasSaveFile();
    void DeleteSave();
}