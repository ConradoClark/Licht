using System;
using System.Linq;
using System.Text;
using Licht.Impl.Orchestration;
using NUnit.Framework;

namespace Licht.Test
{
    [TestFixture]
    public class OrchestrationTests
    {
        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_12345_5Machines()
        {
            var builder = new StringBuilder();
            var machinery = new BasicMachinery<int>(0);
            machinery.AddMachines(1,new BasicMachine(()=> builder.Append("a")),
                new BasicMachine(() => builder.Append("b")),
                new BasicMachine(() => builder.Append("c")),
                new BasicMachine(() => builder.Append("d")),
                new BasicMachine(() => builder.Append("e"))
            );
            machinery.Update();
            Assert.AreEqual("abcde", builder.ToString());

            machinery.Update();
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_12345_1Machine()
        {
            var builder = new StringBuilder();
            var machinery = new BasicMachinery<int>(0);
            machinery.AddMachines(1, new BasicMachine(new Action[]
            {
                () => builder.Append("a"),
                () => builder.Append("b"),
                () => builder.Append("c"),
                () => builder.Append("d"),
                () => builder.Append("e"),
            }));
            machinery.Update();
            machinery.Update();
            machinery.Update();
            machinery.Update();
            machinery.Update();
            Assert.AreEqual("abcde", builder.ToString());
            
            machinery.Update();
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_13254_5Machines()
        {
            var builder = new StringBuilder();
            var machinery = new BasicMachinery<int>(0);
            machinery.AddMachines(1, new BasicMachine( () => builder.Append("a")));
            machinery.AddMachines(3, new BasicMachine(() => builder.Append("b")));
            machinery.AddMachines(2, new BasicMachine(() => builder.Append("c")));
            machinery.AddMachines(5, new BasicMachine(() => builder.Append("d")));
            machinery.AddMachines(4, new BasicMachine(() => builder.Append("e")));

            machinery.SetLayerOrder(Enumerable.Range(1,5).ToArray());

            machinery.Update();
            Assert.AreEqual("acbed", builder.ToString());

            machinery.Update();
        }

        [Test]
        [Parallelizable(ParallelScope.Self)]
        public void Machinery_StringBuilder_12_2Machines_Queued_FIFO()
        {
            StringBuilder builder = new StringBuilder();
            var machinery = new BasicMachinery<int>(0);

            var bq = new FIFOQueue();
            machinery.AddMachinesWithQueue(1, bq, 
                new BasicMachine(() => builder.Append("a")),
                new BasicMachine(() => builder.Append("b")));

            machinery.Update();
            Assert.AreEqual("a", builder.ToString());

            machinery.Update();
            Assert.AreEqual("ab", builder.ToString());

            machinery.Update();
        }
    }
}
