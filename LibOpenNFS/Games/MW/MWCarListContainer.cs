using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibOpenNFS.Core;
using LibOpenNFS.DataModels;
using LibOpenNFS.Utils;
using System.Runtime.InteropServices;

namespace LibOpenNFS.Games.MW
{
    public class MWCarListContainer : Container<CarList>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct CarInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public readonly string IDOne;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public readonly string IDTwo;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public readonly string ModelPath;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public readonly string Maker;

            public readonly uint NameHash;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
            private readonly byte[] padOne;

            public readonly byte CarId;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            private readonly byte[] padTwo;

            public readonly uint TypeHash;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
            private readonly byte[] padThree;

            private readonly byte UnknownFlag;

            public readonly byte SkinsDisabled;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            private readonly byte[] padFour;

            public readonly uint ReflectionConfig;
        }

        public MWCarListContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader, containerSize)
        {
        }

        public override CarList Get()
        {
            if (ContainerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _carList = new CarList(ChunkID.BCHUNK_CARINFO_ARRAY, ContainerSize, BinaryReader.BaseStream.Position);

            ReadChunks(ContainerSize);

            return _carList;
        }

        protected override void ReadChunks(long totalSize)
        {
//            Console.WriteLine("sizeof CarInfo = {0}", Marshal.SizeOf(typeof(CarInfo)));

            BinaryReader.BaseStream.Seek(8, SeekOrigin.Current);
            totalSize -= 8;

            var numCars = (uint) (totalSize / Marshal.SizeOf(typeof(CarInfo)));

            for (var i = 0; i < numCars; i++)
            {
                var carInfo = BinaryUtil.ByteToType<CarInfo>(BinaryReader);

                _carList.Cars.Add(new Car
                {
                    IDOne = carInfo.IDOne,
                    IDTwo = carInfo.IDTwo,
                    Maker = carInfo.Maker,
                    ModelPath = carInfo.ModelPath,
                    NameHash = carInfo.NameHash,
                    CarId = carInfo.CarId,
                    ReflectionConfig = carInfo.ReflectionConfig,
                    SkinsDisabled = carInfo.SkinsDisabled,
                    TypeHash = carInfo.TypeHash
                });

//                Console.WriteLine("Car #{0}: {1} {2} [{3}]", i + 1, carInfo.Maker, carInfo.IDOne, carInfo.ModelPath);
            }
        }

        private CarList _carList;
    }
}
