using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class ImageHaver(Layers layer, Position pos)
{
    public LayerID Layer { get; private set; } = new LayerID(layer);
    public Position Pos { get; private set; } = pos;
    ImageType _image = new();
    public (float X, float Y) SpecificPos { get; private set; } = pos;

    /// <param name="MoveTo"></param>
    /// <returns>true if the pos should be moved</returns>
    bool MoveSpecificTo((float, float) MoveTo, out Position moveTo)
    {
        moveTo = Pos;
        (int, int) oldPos = ((int, int))SpecificPos;
        SpecificPos = MoveTo;

        (int, int) newPos = ((int, int))SpecificPos;
        if (!oldPos.Equals(newPos))
        {
            moveTo = new Position(newPos);
            return true;
        }
        return false;
    }
    /// <summary>
    ///  Changes what is shown on the screen, and also morphes the <see cref="ImageHaver"/> given.
    /// </summary>
    public void ChangeImage(ImageType newImage, bool show = true)
    {
        if (show)
            RemoveImage();
        _image = newImage;
        if (show)
        {
            PrintImage();
        }
    }
    /// <summary>
    /// Changes the image, so it's shaded with the gradient given
    /// </summary>
    public void ChangeImage(GradientType? gradient, bool show = true)
    {
        _image = _image.ChangeGradient(gradient);
        if (show)
            PrintImage(true);
    }

    /// <summary>
    ///  Refreshes the pixels in pixel map
    /// </summary>
    public void PrintImage(bool remove = true)
    {
        if (remove)
            RemoveImage();

        foreach ((Position pos, PixelType pixel) in _image.Image)
        {
            if (!(Pos + pos).IsOutOfBounds())
                (Pos + pos).GetPixel().AddPixel(Layer, pixel);
            //Makes sure that program doesn't break if half of
             //the image would be out of the screen
            //catch (IndexOutOfRangeException) { }
        }

    }
    /// <summary>
    ///  Removes the image from <see cref="Screen"/>
    /// </summary>
    public void RemoveImage()
    {
        foreach ((Position pos, _) in _image.Image)
        {
            if (!(Pos + pos).IsOutOfBounds())
                (Pos + pos).GetPixel().RemovePixel(Layer);
        }
    }
    /// <summary>
    /// Changes the position and specific posiiton to a new position
    /// </summary>
    /// <param name="MoveTo"></param>
    public void MoveTo(Position MoveTo)
    {
        SpecificPos = MoveTo;
        Move(MoveTo);
    }
    /// <summary>
    /// Changes the position to a new position
    /// </summary>
    /// <param name="MoveTo"></param>
    void Move(Position MoveTo)
    {
        RemoveImage();
        Pos = MoveTo;
        PrintImage(false);
    }
    /// <summary>
    /// Changes the real pos to the given one. If the corrasponding position is diffrent 
    /// Moves the image
    /// </summary>
    /// <param name="moveTo"></param>
    public void MoveTo((float, float) moveTo)
    {
        //if The positon its being told to move to would
        //move the image,it does
        if (MoveSpecificTo(moveTo, out Position move))
            Move(move);

    }

    /// <summary>
    ///  Adds the "MoveBy" to the <see cref="ImageHaver"/> position.
    /// </summary>
    /// <param name="MoveBy">
    /// Is the amount the object's being moved by, not it's new position.
    /// </param>
    public void MoveBy(Position MoveBy)
    {
        if (MoveSpecificTo(SpecificPos.Add(MoveBy), out Position moveTo))
            MoveTo(moveTo);
    }
    /// <summary>
    ///  Adds the "MoveBy" to the <see cref="ImageHaver"/>'s <see cref="Position.realPos"/> .
    ///  If That would change the <see cref="Position.Pos"/>, It does.
    /// </summary>
    /// <param name="MoveBy">
    ///  Is the change of the realPosition
    /// </param>
    public void MoveBy((float, float) MoveBy)
    {
        if (MoveSpecificTo(SpecificPos.Add(MoveBy), out Position newPos))
            Move(newPos);
    }

    /// <summary>
    ///  Creats a new ImageHaver and imideatly print the image given
    /// </summary>
    /// <param name="image"></param>
    /// <param name="layer"></param>
    /// <param name="pos"></param>
    public ImageHaver(ImageType image,
                 Layers layer, Position pos) : this(layer, pos) => ChangeImage(image);
}
