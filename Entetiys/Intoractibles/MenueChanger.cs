using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class MenueChanger : Intoractible
{
    public MenueChanger(Position pos, Position fromCenter, MenueType menue, Image image, Layers layer) : base(pos, fromCenter,layer)
    {
        Frame.ChangeImage(image);
        this.menue = menue;
        Show();
    }

    public MenueChanger(Position pos,Position fromCenter,MenueType menue, Image image): base(pos,fromCenter)
    {
        Frame.ChangeImage(image);
        this.menue = menue;
        Show();
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
    }
    public MenueChanger(Position pos, string text, MenueType menue) : this(pos,new Position(0,0),text,menue) {     }
    public MenueChanger(Position pos, Position fromCenter, string text, MenueType menue) : this(pos, fromCenter,menue, new Image(text,BordInfo.GreenText)) { }
    public MenueChanger(Position pos, Position fromCenter, string text, MenueType menue, (ColorType back,ColorType front, ColorType doubleBack) colors) : base(pos, fromCenter)
    {
        this.menue = menue;
        Frame.ChangeImage(new Image(text, colors), shown);
        Show();       
    }
    readonly MenueType menue;


    public override bool BeActedOn<T>(T d)
    {
        if (d is Pressed)
        {
            Scene.Instance.SendChangeMenueMessage(menue);
            return true;
        }
        else
            return false;
    }
}
public class MessageSender :Intoractible
{
    Message message;
    object? data;

    public MessageSender(Position pos, Position fromCenter, (Message message, object? data) message, Image image, Layers layer) : base(pos, fromCenter, layer)
    {
        Frame.ChangeImage(image);
        this.message = message.message;
        this.data = message.data;
        Show();
    }
    public MessageSender(Position pos, Position fromCenter, (Message message, object? data) message, Image image) : base(pos, fromCenter)
    {
        Frame.ChangeImage(image);
        this.message = message.message;
        this.data = message.data;
        Show();
    }
    public override bool BeActedOn<T>(T d)
    {
        if (d is Pressed)
        {
            Scene.Instance.SendMessage(message, data);
            return true;
        }
        else
            return false;
    }

}

public class FunctionCaller : Intoractible
{
    public FunctionCaller(Position pos, Position fromCenter, Action action, Image image, Layers layer) : base(pos, fromCenter, layer)
    {
        Frame.ChangeImage(image);
        this.action = action;
        Show();
    }
    public FunctionCaller(Position pos, Position fromCenter, Action action) : base(pos, fromCenter)
    {
        Frame.ChangeImage(new Image("Function", BordInfo.GreenText));
        this.action = action;
        Show();
    }
    readonly Action action;
    public override bool BeActedOn<T>(T d)
    {
        if (d is Pressed)
        {
            action.Invoke();
            return true;
        }
        else
            return false;
    }

}