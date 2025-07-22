using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;


public interface IEntity
{
    LayerID Layer { get; }
    /// <summary>
    ///  Functions as the method all entity's take their own actions.
    /// </summary>
    /// <returns>A bool who's function is decided by the Inheritor</returns>
    void TakeAction();
    bool BeActedOn<T>(T d) where T : IAction;
    void Destroy();

}

