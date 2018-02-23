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
        struct CarInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string IDOne;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string IDTwo;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string ModelPath;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string Maker;

            public uint NameHash;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
            public byte[] padOne;

            public byte CarId;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] padTwo;

            public uint TypeHash;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
            public byte[] padThree;

            public byte UnknownFlag;

            public byte SkinsDisabled;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] padFour;

            public uint ReflectionConfig;
        }

        public MWCarListContainer(BinaryReader binaryReader, long? containerSize) : base(binaryReader, containerSize)
        {
        }

        public override CarList Get()
        {
            if (containerSize == 0)
            {
                throw new Exception("containerSize is not set!");
            }

            _carList = new CarList((long) ChunkID.BCHUNK_CARINFO_ARRAY, containerSize);

            ReadChunks(containerSize);

            return _carList;
        }

        protected override uint ReadChunks(long TotalSize)
        {
            Console.WriteLine("sizeof CarInfo = {0}", Marshal.SizeOf(typeof(CarInfo)).ToString());

            binaryReader.BaseStream.Seek(8, SeekOrigin.Current);
            TotalSize -= 8;

            uint numCars = (uint) (TotalSize / Marshal.SizeOf(typeof(CarInfo)));

            for (int i = 0; i < numCars; i++)
            {
                CarInfo carInfo = BinaryUtil.ByteToType<CarInfo>(binaryReader);

                _carList.Cars.Add(new Car
                {
                    IDOne = carInfo.IDOne,
                    IDTwo = carInfo.IDTwo,
                    Maker = carInfo.Maker,
                    ModelPath = carInfo.ModelPath
                });

                Console.WriteLine("Car #{0}: {1} {2} [{3}]", i + 1, carInfo.Maker, carInfo.IDOne, carInfo.ModelPath);
            }

            return 0;
        }

        private CarList _carList;
    }
}
