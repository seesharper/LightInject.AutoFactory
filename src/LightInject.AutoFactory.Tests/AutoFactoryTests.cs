using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightInject.xUnit2;
using Xunit;

namespace LightInject.AutoFactory.Tests
{
    public class AutoFactoryTests
    {
        [Theory, InjectData]
        public void ShoudGetInstanceUsingFactory(IFooFactory fooFactory)
        {
            var instance = fooFactory.GetFoo(42);

            Assert.IsType<Foo>(instance);
        }

        [Theory, InjectData]
        public void ShouldGetNamedInstanceUsingFactory(IFooFactory fooFactory)
        {         
            var instance = fooFactory.GetAnotherFoo(42);

            Assert.IsType<AnotherFoo>(instance);
        }

        [Theory, InjectData]
        public void ShouldGetGenericInstanceUsingFactory(IFooFactory fooFactory)
        {
            var instance = fooFactory.GetFoo<Disposable>(42);

            Assert.IsType<Foo<Disposable>>(instance);
        }
      
        [Theory, InjectData]
        public void ShouldGetConcreteInstanceUsingFactory(IAnotherFooFactory fooFactory)
        {
            var foo = fooFactory.GetFoo(42);
            Assert.IsType<Foo>(foo);

            var anotherFoo = fooFactory.GetAnotherFoo(42);
            Assert.IsType<AnotherFoo>(anotherFoo);
        }
      
        [Fact]
        public void ShouldThrowMeaningfulExceptionWhenFactoryIsNotAnInterface()
        {
            AutoFactoryBuilder builder = new AutoFactoryBuilder(new TypeBuilderFactory(), new ServiceNameResolver());
            Assert.Throws<InvalidOperationException>(() => builder.GetFactoryType(typeof(Foo)));            
        }

        public static void Configure(IServiceContainer container)
        {
            container.Register<int, IFoo>((factory, value) => new Foo(value));
            container.Register<int, Foo>((factory, value) => new Foo(value));
            container.Register<int, AnotherFoo>((factory, value) => new AnotherFoo(value));            
            container.Register(typeof(IFoo<>), typeof(Foo<>));            
            container.RegisterConstructorDependency((factory, info, args) => (int)args[0]);
            container.Register<int, IFoo>((factory, value) => new AnotherFoo(value), "AnotherFoo");            
            container.EnableAutoFactories();
            container.RegisterAutoFactory<IFooFactory>();
            container.RegisterAutoFactory<IAnotherFooFactory>();            
        }

        internal virtual AutoFactoryBuilder CreateFactoryBuilder()
        {
            return new AutoFactoryBuilder(new TypeBuilderFactory(),  new ServiceNameResolver());
        }
    }


    public interface IFooFactory
    {
        IFoo GetFoo(int value);

        IFoo GetAnotherFoo(int value);

        IFoo<T> GetFoo<T>(int value) where T: Disposable, IDisposable;        
    }

    public interface IAnotherFooFactory
    {
        Foo GetFoo(int value);

        AnotherFoo GetAnotherFoo(int value);
    }

    public interface IFoo { }



    public class Foo : IFoo
    {
        public Foo(int value)
        {
        }
    }

    public interface IFoo<T> where T:Disposable, IDisposable
    {
        
    }

    public class Disposable : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class Foo<T> : IFoo<T> where T:Disposable, IDisposable
    {
        public Foo(int value)
        {
        }
    }

        
    public class FooWithInterfaceConstraint<T> where T : IDisposable
    {
        public FooWithInterfaceConstraint(int value)
        {
        }
    }


    public class AnotherFoo : IFoo
    {
        public AnotherFoo(int value)
        {
        }
    }
  
    public class FooFactory : IFooFactory
    {
        private readonly IServiceFactory serviceFactory;

        public FooFactory(IServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory;
        }

        public IFoo GetFoo(int value )
        {
            return serviceFactory.GetInstance<int, IFoo>(value);
        }

        public IFoo GetAnotherFoo(int value)
        {
            return serviceFactory.GetInstance<int, IFoo>(value, "AnotherFoo");
        }

        public IFoo<T> GetFoo<T>(int value) where T:Disposable, IDisposable
        {
            return serviceFactory.GetInstance<int, IFoo<T>>(value);
        }     
    }

   
}
