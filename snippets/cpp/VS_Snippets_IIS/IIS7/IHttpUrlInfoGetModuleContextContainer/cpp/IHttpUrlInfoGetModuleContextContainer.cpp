// <Snippet1>
// The MyContainer class implements the 
// IDispensedHttpModuleContextContainer interface.
class MyContainer : public IDispensedHttpModuleContextContainer
{
public:
    // The MyContainer method is the public
    // constructor for the MyContainer class.
    // Make this method protected if the 
    // MyContainer class is abstract.
    // dispensed: true if the container should
    // call delete this when the ReleaseContainer
    // method is called.
    MyContainer(bool dispensed = false) 
        : m_dispensed(dispensed)
    {
        
    }

    // The ReleaseContainer method 
    // calls delete this if this container
    // is dispensed.
    virtual VOID ReleaseContainer(VOID)
    {
        if (m_dispensed)
        {
            delete this;
        }
    }

    // Implement additional 
    // IDispensedHttpModuleContextContainer
    // pure virtual methods if this class
    // is not abstract.

private:
    // The MyContainer method is the private
    // destructor for the MyContainer class.
    // Make this method protected and virtual 
    // if the MyContainer class expects 
    // to be a class of derivation. This method 
    // should not be public because 
    // IDispensedHttpModuleContextContainer pointers
    // should be disposed externally only by 
    // calling the ReleaseContainer method.
    ~MyContainer()
    {

    }

    // Specify a Boolean value for dispensing.
    bool m_dispensed;
};

// The MyClass class implements the
// IHttpUrlInfo interface.
class MyClass : public IHttpUrlInfo
{
public:
    // The MyClass method is the public
    // constructor for the MyClass class.
    MyClass()
    {
        m_container = new MyContainer;
    }

    // The MyClass method is the 
    // public virtual destructor 
    // for the MyClass class. This destructor
    // calls ReleaseContainer on the internal
    // IDispensedHttpModuleContextContainer
    // pointer and sets that pointer to NULL.
    virtual ~MyClass()
    {
        m_container->ReleaseContainer();
        m_container = NULL;
    }

    // The GetModuleContextContainer method
    // returns an IHttpModuleContextContainer
    // pointer.
    // return: an explicit upcast 
    // IDispensedHttpModuleContextContainer
    // pointer for readability.
    virtual IHttpModuleContextContainer* 
        GetModuleContextContainer(VOID)
    {
        return (IHttpModuleContextContainer*)m_container;
    }
 
    // Implement additional IHttpUrlInfo
    // pure virtual methods if this class
    // is not abstract.

private:
    // Specify a private
    // IDispensedHttpModuleContextContainer
    // pointer.
    IDispensedHttpModuleContextContainer* m_container;
};
// </Snippet1>