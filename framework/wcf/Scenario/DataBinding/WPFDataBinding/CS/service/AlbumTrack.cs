//  Copyright (c) Microsoft Corporation. All rights reserved.

using System.Runtime.Serialization;

namespace Microsoft.Samples.DataBinding
{
    [DataContract(Namespace = "http://Microsoft.Samples.DataBinding")]
    public class Album
    {
        [DataMember]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        [DataMember]
        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        [DataMember]
        public Track[] Tracks
        {
            get { return tracks; }
            set { tracks = value; }
        }

        private string title;
        private double price;
        private Track[] tracks;
    }



    [DataContract(Namespace = "http://Microsoft.Samples.DataBinding")]
    public class Track
    {
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [DataMember]
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        private string name;
        private int duration;
    }
}
