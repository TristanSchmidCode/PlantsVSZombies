using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace PlantsVSZombies;

public class EntityHanderler: SingleTone<EntityHanderler>
{
    class EmptyEntity : IEntity
    {
        public LayerID Layer => new(Layers.Background);
        public void TakeAction() { }
        public bool BeActedOn<T>(T d) where T : IAction => true;
        public void Destroy() { }
    }
    static EmptyEntity _emptyEntity = new EmptyEntity();
    readonly List<IEntity> _entities = [];
    //a list of entities that are to be registered, this is used to avoid registering entities while iterating through the _entities list
    readonly List<IEntity> _entitiesToAdd = [];
    //a list of indexes of entities that are to be removed, this is used to avoid removing entities while iterating through the _entities list
    readonly List<int> _entitiesToDelete = [];
    //a dictionary of LayerID and the index of the entity in the _entities list, used for fast access to entities
    readonly Dictionary<LayerID, int> _keyValuePairs = [];
    public IEntity? GetEntity(LayerID? layer)
    {
        if (layer is null)
            return null;
        if (!_keyValuePairs.TryGetValue(layer.Value, out int index))
        {
            // if the entity is not found in the entities list, throw an exception
            throw new KeyNotFoundException($"No entity found with layer {layer}");
        }
        return _entities[index];
    }
    void UnRegistorEntity(int index)
    {
        if (_entities.Count == 1) // if there is only one entity, it will be removed and the list cleared
        {
            _entities.Clear();
            _keyValuePairs.Clear();
            return;
        }
        if (_entities.Count - 1 == index) // if the entity is the last one in the list, it will be removed and the list will be shortened
        {
            _entities.RemoveAt(index);
            return;
        }

        //swaps the entity with the last one in the list and removes it
        IEntity lastEntity = _entities[^1];
        _entities[index] = lastEntity;
        _entities.RemoveAt(_entities.Count - 1);
        _keyValuePairs[lastEntity.Layer] = index;
    }
    void RegistorEntity(IEntity entity)
    {
        if (_keyValuePairs.TryGetValue(entity.Layer, out _))
            return; // if the entity is already registered, do nothing
        
        _keyValuePairs.Add(entity.Layer, _entities.Count );
        _entities.Add(entity);
    }
    //adds all registering entities and removes all unRegistering entities
    public void RegisterAndUnrgisterAll()
    {
        //registers all registering entities by the order they were added
        for (int i = 0; i < _entitiesToAdd.Count; i++)
           RegistorEntity(_entitiesToAdd[i]);
        _entitiesToAdd.Clear();
        //unregisters all unRegistering entities by the order they were added

        if (_entitiesToDelete.Count == 0)
            return; // if there are no entities to delete, do nothing
        //sorts the indexes in descending order, so the indexes don't change while removing entities
        _entitiesToDelete.Sort((a, b) => b.CompareTo(a));
        for (int i = 0; i < _entitiesToDelete.Count; i++)
        {
           UnRegistorEntity(_entitiesToDelete[i]);

        }
        _entitiesToDelete.Clear();
    }
    
    public void ActivateAll()
    {
        int a = _entities.Count;
        for (int i = 0; i < _entities.Count; i++)
        {
            _entities[i].TakeAction();
            if (a != _entities.Count)
                return;
        }
    }
    //puts the entity in a list to be added next frame
    public void AddEntity(IEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        if (_keyValuePairs.TryGetValue(entity.Layer, out _))
            return;
        _entitiesToAdd.Add(entity);
    }
    //puts the entity ina list to be removed next frame
    public void RemoveEntity(IEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        if (!_keyValuePairs.TryGetValue(entity.Layer, out int index))
            _entitiesToAdd.Remove(entity); // if the entity is not registered, remove it from the registering list if its there
        else
        {
            _entitiesToDelete.Add(index);
            //put a dummy entity in the place of the removed entity, so the index doesn't change
            _entities[index] = _emptyEntity;
            //removes the entity from the keyValuePairs dictionary, to make sure the empty entity isn't acsessed
            _keyValuePairs.Remove(entity.Layer);
        }
    }

    protected override void DestroyThis()
    {
        for (int i = _entities.Count - 1; i >= 0; i--)
        {
            IEntity entity = _entities[i];
            entity.Destroy();
        }
        //destroys all registering entities
        for (int i = _entitiesToAdd.Count - 1; i >= 0; i--)
        {
            IEntity entity = _entitiesToAdd[i];
            entity.Destroy();
            
        }
        _entitiesToAdd.Clear();
        _entities.Clear();
        _keyValuePairs.Clear();
    }
}

