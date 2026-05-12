using System;

[Serializable]
public class UserCreate
{
    public string username;
}

[Serializable]
public class RunCreate
{
    public int score;
    public int duration;
    public string username;
}

[Serializable]
public class RunRead
{
    public int score;
    public int duration;
    public int id;
    public string username;
    public string created_at;
}

[Serializable]
public class RunReadList
{
    public RunRead[] items;
}

