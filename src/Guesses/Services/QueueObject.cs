namespace PalpiteFC.Worker.Guesses.Services;

public class QueueObject<T>
{
    public int Tries { get; set; }
    public DateTime LastTry { get; set; }
    public T Data { get; set; }

    public QueueObject(T data)
    {
        Data = data;
    }
}