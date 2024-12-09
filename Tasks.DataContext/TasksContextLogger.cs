using static System.Environment;

namespace Tasks.EntityModels;

//TO DO: Encontrar alternativa para despliegue en web 
public class TasksContextLogger
{
  public static void WriteLine(string message)
  {
    
    string path = Path.Combine(Environment.CurrentDirectory, "tasks.txt");

    StreamWriter textFile = File.AppendText(path);
    textFile.WriteLine(message);
    textFile.Close();
  }
}