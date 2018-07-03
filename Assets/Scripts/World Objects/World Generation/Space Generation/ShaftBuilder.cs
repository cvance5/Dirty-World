using System;
using WorldObjects;
using WorldObjects.WorldGeneration;

public class ShaftBuilder : SpaceBuilder
{
    private IntVector2 _top;
    private IntVector2 _middle;
    private IntVector2 _bottom;
    private ShaftAlignment _alignment;

    private int _height;
    private int _width;

    public ShaftBuilder(IntVector2 startingPoint, ShaftAlignment alignment)
    {
        switch (alignment)
        {
            case ShaftAlignment.StartFromTop: _top = startingPoint; break;
            case ShaftAlignment.StartFromMiddle: _middle = startingPoint; break;
            case ShaftAlignment.StartFromBelow: _bottom = startingPoint; break;
        }

        _alignment = alignment;
        FindAllPoints();
    }

    public ShaftBuilder SetLength(int blocksLong)
    {
        _width = blocksLong - 1;
        FindAllPoints();
        return this;
    }

    public ShaftBuilder SetHeight(int blockHigh)
    {
        _height = blockHigh - 1;
        FindAllPoints();
        return this;
    }

    public override SpaceBuilder Clamp(IntVector2 direction, int amount)
    {
        if (direction == Directions.Up)
        {
            _top.Y = Math.Min(_top.Y, amount);
            _alignment = ShaftAlignment.StartFromTop; // We have to enforce this boundary
        }
        else if (direction == Directions.Right)
        {
            if (_bottom.X + _width > amount)
            {
                var difference = (_bottom.X + _width) - amount;
                _bottom.X -= difference;
                _middle.X -= difference;
                _top.X -= difference;
            }
        }
        else if (direction == Directions.Down)
        {
            _bottom.Y = Math.Max(_bottom.Y, amount);
            _alignment = ShaftAlignment.StartFromBelow; // We have to enforce this boundar
        }
        else if (direction == Directions.Left)
        {
            if (_bottom.X < amount)
            {
                _bottom.X = amount;
                _middle.X = amount;
                _top.X = amount;
            }
        }
        else throw new ArgumentException($" Expected a cardinal direction.  Cannot operate on {direction}.");

        FindAllPoints();
        return this;
    }

    public override Space Build() => new Shaft(_bottom, new IntVector2(_top.X + _width, _top.Y));

    private void FindAllPoints()
    {
        switch (_alignment)
        {
            case ShaftAlignment.StartFromTop:
                if (_top == null) throw new System.InvalidOperationException("Corridor builder has not been assigned a left position.");
                else
                {
                    _middle = new IntVector2(_top.X, _top.Y - (_height / 2));
                    _bottom = new IntVector2(_top.X, _top.Y - _height);
                }
                break;
            case ShaftAlignment.StartFromMiddle:
                if (_middle == null) throw new System.InvalidOperationException("Corridor builder has not been assigned a central position.");
                else
                {
                    _top = new IntVector2(_middle.X, _middle.Y + (_height / 2));
                    _bottom = new IntVector2(_middle.X, _middle.Y - (_height / 2));
                }
                break;
            case ShaftAlignment.StartFromBelow:
                if (_bottom == null) throw new System.InvalidOperationException("Corridor builder has not been assigned a right position.");
                else
                {
                    _middle = new IntVector2(_bottom.X, _bottom.Y + (_height / 2));
                    _top = new IntVector2(_bottom.X, _bottom.Y + _height);
                }
                break;
        }
    }

    public enum ShaftAlignment
    {
        StartFromTop,
        StartFromMiddle,
        StartFromBelow
    }
}
