public interface IAssembly
{
    void Init(EntityAssembly owner);
    void Release();
}

public abstract class AssemblyBase : IAssembly
{

    [Newtonsoft.Json.JsonIgnore] private EntityAssembly _owner;
    [Newtonsoft.Json.JsonIgnore] public EntityAssembly Owner { get { return _owner; } }

    public void Init(EntityAssembly owner)
    {
        _owner = owner;
        OnInit(owner);
    }
    public void Release()
    {
        OnRelease();
        _owner = null;
    }
    protected virtual void OnInit(EntityAssembly owner)
    {

    }
    /// <summary>
    /// ���л��������
    /// </summary>
    public virtual void ReadDataFinish()
    {

    }

    protected virtual void OnRelease()
    {

    }


}