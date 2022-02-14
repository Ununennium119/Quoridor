using System;
using UnityEngine;

/// <summary>
/// Class <c>Position</c> models a position in two-dimensional XZ plane.
/// </summary>
[Serializable]
public class Position
{
    [SerializeField] private int x;
    [SerializeField] private int z;

    /// <value>Represents X component of the position.</value>
    public int X
    {
        get => x;
        private set => x = value;
    }

    /// <value>Represents Z component of the position.</value>
    public int Z
    {
        get => z;
        private set => z = value;
    }

    /// <summary>
    /// Initializes a new instance of the <c>Position</c> class.
    /// </summary>
    /// <param name="x">Position's X component</param>
    /// <param name="z">Position's Z component</param>
    public Position(int x, int z)
    {
        X = x;
        Z = z;
    }


    /// <param name="direction"></param>
    /// <returns>Adjacent position in given <paramref name="direction"/>.</returns>
    public Position GetDirectionPosition(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Up(),
            Direction.Right => Right(),
            Direction.Down => Down(),
            _ => Left()
        };
    }

    /// <returns>Adjacent position in upward direction.</returns>
    public Position Up()
    {
        return new Position(x, z + 1);
    }

    /// <returns>Adjacent position in downward direction.</returns>
    public Position Down()
    {
        return new Position(x, z - 1);
    }

    /// <returns>Adjacent position in rightward direction.</returns>
    public Position Right()
    {
        return new Position(x + 1, z);
    }

    /// <returns>Adjacent position in leftward direction.</returns>
    public Position Left()
    {
        return new Position(x - 1, z);
    }
}