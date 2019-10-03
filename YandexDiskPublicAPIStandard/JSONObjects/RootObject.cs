using System;
using System.Collections.Generic;

namespace YandexDiskPublicAPI.JSONObjects
{
    class RootObject
    {
        public string public_key { get; set; }
        public Embedded _embedded { get; set; }
        public string name { get; set; }
        public Exif2 exif { get; set; }
        public DateTime created { get; set; }
        public string resource_id { get; set; }
        public string public_url { get; set; }
        public DateTime modified { get; set; }
        public int views_count { get; set; }
        public Owner owner { get; set; }
        public string path { get; set; }
        public CommentIds2 comment_ids { get; set; }
        public string type { get; set; }
        public long revision { get; set; }
    }

    class Exif
    {
    }

    class CommentIds
    {
        public string private_resource { get; set; }
        public string public_resource { get; set; }
    }

    class Item
    {
        public string antivirus_status { get; set; }
        public string public_key { get; set; }
        public string sha256 { get; set; }
        public string name { get; set; }
        public Exif exif { get; set; }
        public DateTime created { get; set; }
        public long revision { get; set; }
        public string resource_id { get; set; }
        public DateTime modified { get; set; }
        public CommentIds comment_ids { get; set; }
        public string file { get; set; }
        public string media_type { get; set; }
        public string path { get; set; }
        public string md5 { get; set; }
        public string type { get; set; }
        public string mime_type { get; set; }
        public int size { get; set; }
    }

    class Embedded
    {
        public string sort { get; set; }
        public string public_key { get; set; }
        public List<Item> items { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public string path { get; set; }
        public int total { get; set; }
    }

    class Exif2
    {
    }

    class Owner
    {
        public string login { get; set; }
        public string display_name { get; set; }
        public string uid { get; set; }
    }

    class CommentIds2
    {
        public string private_resource { get; set; }
        public string public_resource { get; set; }
    }
}
