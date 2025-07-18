using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class MenueChanger : Intoractible
{
    public MenueChanger(Position pos, Position fromCenter, MenueType menue, ImageType image, Layers layer) : base(pos, fromCenter,layer)
    {
        frame.ChangeImage(image);
        this.menue = menue;
        Show();
        Scene.OnMenueChange += Destroy;
    }

    public MenueChanger(Position pos,Position fromCenter,MenueType menue, ImageType image): base(pos,fromCenter)
    {
        frame.ChangeImage(image);
        this.menue = menue;
        Show();
        Scene.OnMenueChange += Destroy;
    }
    void Destroy(MenueType menue)
    {
        if (_canBeDestroyed)
            Destroy();
        _canBeDestroyed = true;
    }
    bool _canBeDestroyed = false;
    public MenueChanger(MenueType menue):base((0,0),(0,0))
    {
        this.menue = menue;
        Scene.OnMenueChange += Destroy;
    }

    public MenueChanger(Position pos, string text, MenueType menue) : this(pos,new Position(0,0),text,menue) {     }
    public MenueChanger(Position pos, Position fromCenter, string text, MenueType menue) : this(pos, fromCenter,menue, new ImageType(text,BordInfo.GreenText)) { }
    public MenueChanger(Position pos, Position fromCenter, string text, MenueType menue, (ColorType back,ColorType front, ColorType doubleBack) colors) : base(pos, fromCenter)
    {
        this.menue = menue;
        frame.ChangeImage(new ImageType(text, colors), shown);
        Show();
        Scene.OnMenueChange += Destroy;
        
    }
    readonly MenueType menue;


    public override bool BeActedOn<T>(T d)
    {
        if (d is Pressed)
        {
            Scene.ChangeMenue(menue);
            return true;
        }
        else
            return false;
    }

    public override void TakeAction()
    {
        throw new NotImplementedException();
    }

}
