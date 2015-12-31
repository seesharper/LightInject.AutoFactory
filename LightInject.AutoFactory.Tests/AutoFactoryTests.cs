﻿using System;
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

        [Fact]
        public void ShouldThrowMeaningfulExceptionWhenFactoryIsNotAnInterface()
        {
            AutoFactoryBuilder builder = new AutoFactoryBuilder(new TypeBuilderFactory(), new ServiceNameResolver());
            Assert.Throws<InvalidOperationException>(() => builder.GetFactoryType(typeof(Foo)));            
        }

        public static void Configure(IServiceContainer container)
        {
            container.Register<int, IFoo>((factory, value) => new Foo(value));                                    
            container.EnableAutoFactories();
            container.RegisterAutoFactory<IFooFactory>();
        }

        protected virtual AutoFactoryBuilder CreateFactoryBuilder()
        {
            return new AutoFactoryBuilder(new TypeBuilderFactory(),  new ServiceNameResolver());
        }
    }


    public interface IFooFactory
    {
        IFoo GetFoo(int value);

        IFoo GetAnotherFoo(int value);
    }

    public interface IFoo { }

    public class Foo : IFoo
    {
        public Foo(int value)
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
    }
}
