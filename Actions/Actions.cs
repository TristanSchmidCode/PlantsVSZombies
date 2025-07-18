using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies
{
    public interface IAction
    {
        Layers[] IntoractsWith { get; init; }
    }

    public readonly struct PickUP(Layers intoractsWith) : IAction
    {
        public Layers[] IntoractsWith { get; init; } = [intoractsWith];
    }

    public readonly struct Pressed(MenueType situation) : IAction
    {
        public readonly MenueType situation = situation;
        public Layers[] IntoractsWith { get; init; } = [];
    }
    public readonly struct Move(ConsoleKey direction) : IAction
    {
        public readonly ConsoleKey direction = direction;
        public Layers[] IntoractsWith { get; init; } = [];

    }
    public readonly struct Attack : IAction
    {
        public  Layers[] IntoractsWith { get; init; }
        public readonly int damage;
       
        public Attack(Layers intoractsWith, int damage)
        {
            this.damage = damage;
            this.IntoractsWith = [ intoractsWith ];
        }
        public Attack(Layers[] intoractsWith, int damage)
        {
            this.damage = damage;
            this.IntoractsWith = intoractsWith;     
        }
        


    }
    public class Bullet : ImageHaver, IEntity
    {
        protected int movementSpeed;
        protected int Direction;
        public Bullet(Position pos, int damage, bool positive = true) : base(Layers.Attack, pos)
        {
            attack = new([Layers.BZombie, Layers.SZombie], damage);
            if (positive)
                Direction = 1;
            else
                Direction = -1;

            movementSpeed = 12;
            List<(Position, PixelType)> building =
            [
                ((1,0), new(Colors.White)),
                ((0,0), new(Colors.White,Colors.Black,'\u25A0'))
            ];
            ChangeImage(new ImageType(building));
            AllEntitys.AddEntities(this);
        }

        public Attack attack;

        public virtual void TakeAction()
        {
            Position oldPos = Pos;
            MoveBy((Time.DeltaTime * movementSpeed * Direction, 0));
            if (SpecificPos.X >= BordInfo.MapLeft)
            {
                Destroy();
                return;
            }
            while (oldPos.X * Direction < Pos.X * Direction)
            {
                //if this returns true, it hit an object, and shouldn't contiue to try and find one
                if (Intoracted(oldPos.GetPixel()[attack.IntoractsWith]))
                    { break; }
                oldPos += (Direction, 0);
            } 

        }

        public bool BeActedOn<T>(T d) where T : IAction
        {
            throw new NotImplementedException();
        }
        
        /// <param name="TheHit"></param>
        /// <returns> true if sucsesfully found a target</returns>
        protected virtual bool Intoracted(LayerID? TheHit)
        {
            try
            {
                if (TheHit is LayerID id)
                {
                    //gives the target an attack
                    AllEntitys.Getall()[id].BeActedOn(attack);
                    //destroyes the bullet
                    Destroy();
                    return true;
                }
            }catch (KeyNotFoundException)
            { }
            return false;
        }

        public void Destroy()
        {
            AllEntitys.RemoveEntities(this);
            RemoveImage();
        }
    }
    public readonly struct Buff : IAction 
    {
        public Layers[] IntoractsWith { get; init; }
    
    }
}
