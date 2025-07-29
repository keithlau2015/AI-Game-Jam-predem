using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public abstract class Model<T> : INotifyPropertyChanged where T : class {
    public object key;
    public static SortedDictionary<object, T> map { get; protected set;} = new SortedDictionary<object, T>();

    //For creating new
    public Model() { 
        
    }

    //For loading
    public Model(object key)
    {
        this.key = key;
        map.Add(key, this as T);
    }

    #region Notify Property Changed
    public event PropertyChangedEventHandler PropertyChanged;
    // Create the OnPropertyChanged method to raise the event
    // The calling member's name will be used as the parameter.
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion
}