//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Collections;
using System.ServiceModel;

namespace Microsoft.Samples.DataBinding
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class AlbumService : IAlbumService
    {
        private AlbumService()
        {
            albums = CreateList(5);
        }

        public Album[] GetAlbumList()
        {
            Album[] arr = new Album[albums.Count];
            albums.CopyTo(arr);
            return arr;
        }

        public void AddAlbum(string title)
        {
            Album a = CreateAlbum(title);
            albums.Add(a);
        }

        private ArrayList CreateList(int numOfAlbums)
        {
            ArrayList list = new ArrayList();

            for (int i = 0; i < numOfAlbums; i++)
            {
                Album a = CreateAlbum("Title" + i.ToString());
                list.Add(a);
            }
            return list;
        }

        private Album CreateAlbum(string title)
        {
            Album a = new Album();
            a.Title = title;
            a.Price = 12 + rnd.Next(-4, +6);
            ArrayList tracks = new ArrayList();
            int maxTrack = 4 + rnd.Next(-2, +2);
            for (int i = 0; i < maxTrack; i++)
            {
                Track t = new Track();
                t.Name = "Track-" + title + "-" + i.ToString();
                t.Duration = 100 + rnd.Next(-50, +100); ;
                tracks.Add(t);
            }
            Track[] arr = new Track[tracks.Count];
            tracks.CopyTo(arr);
            a.Tracks = arr;
           
            return a;
        }

        ArrayList albums;
        Random rnd = new Random();
    }
}
