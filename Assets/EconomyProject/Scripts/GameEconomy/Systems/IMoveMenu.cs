using Unity.MLAgents;

public interface IMoveMenu<T> where T : Agent
{
    void MovePosition(T agent, int movement);
}
