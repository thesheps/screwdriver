namespace Screwdriver.Mocking.Tests.TestClasses
{
    public interface IMockableInterfaceA
    {
        void DoTheThing();
        void DoTheThing(int i);
        void DoTheThing(string str);
        void DoTheThing(object obj);
        string GetTheThing();
    }
}