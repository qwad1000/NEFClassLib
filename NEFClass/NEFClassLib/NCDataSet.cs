using System;
using System.Collections.Generic;
using System.IO;
using NEFClassLib.Partitions;

namespace NEFClassLib
{
    public class NCEntity
    {
        private string mClass;
        private double[] mAttributes;

        public NCEntity(double[] attributes, string entityClass)
        {
            int size = attributes.Length;
            this.mAttributes = new double[size];
            for (int i = 0; i < size; ++i)
                this.mAttributes[i] = attributes[i];

            this.mClass = entityClass;
        }

        public double this[int index]
        {
            get { return this.mAttributes[index]; }
        }

        public string Class
        {
            get { return mClass; }
        }

        public int Dimension
        {
            get { return this.mAttributes.Length; }
        }
    }


	//TODO: implement enumerator
    public class NCDataSet
    {
        private NCEntity[] mEntities;

        public NCDataSet(string filename)
        {
            string[] rawData = File.ReadAllLines(filename);

            this.mEntities = new NCEntity[rawData.Length];
            for (int i = 0; i < rawData.Length; ++i)
            {
                string[] entityData = rawData[i].Split(",".ToCharArray());
                double[] attributes = new double[entityData.Length - 1];

                int classIndex = entityData.Length - 1;
                string entityClass = entityData[classIndex];

                for (int j = 0; j < attributes.Length; ++j) {
                    //attributes [j] = Double.TryParse (entityData [j]);//.ToDouble(entityData[j].Replace('.', ','));
                    double parced = 0.0;
                    if (Double.TryParse (entityData [j], out parced))
                        attributes [j] = parced;
                }
                if (attributes.Length > 0) {
                    this.mEntities [i] = new NCEntity (attributes, entityClass);
                }
            }
        }

        public int Dimension
        {
            get { return mEntities[0].Dimension; }
        }

        public NCEntity this[int i]
        {
            get { return mEntities[i]; }
        }

        public int Length
        {
            get { return mEntities.Length; }
        }

        public Bounds GetAttributeBounds(int attributeIndex)
        {
            double minValue = mEntities[0][attributeIndex];
            double maxValue = mEntities[0][attributeIndex];

            for (int i = 1; i < mEntities.Length; ++i)
            {
                if (mEntities [i].Dimension > attributeIndex) {
                    minValue = Math.Min (minValue, mEntities [i] [attributeIndex]);
                    maxValue = Math.Max (maxValue, mEntities [i] [attributeIndex]);
                } else {
                    System.Console.WriteLine ("Problem");
                }
            }

            return new Bounds(minValue, maxValue);
        }

        public string[] GetClassesList()
        {
            List<string> classesList = new List<string>();

            for (int i = 0; i < mEntities.Length; ++i)
                if (!classesList.Contains(mEntities[i].Class))
                    classesList.Add(mEntities[i].Class);

            return classesList.ToArray();
        }
    }
}
