using Zenfolio7.Zenfolio;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Zenfolio7.Model;

namespace Zenfolio7.DataAccess
{
    /// <summary>
    /// A data source that provides raw data objects.  In a real
    /// application this class would make calls to a database.
    /// </summary>
    public static class Database
    {
        public static TreeNode RootNode { get; private set; }
        public static ZenfolioClient Client = new ZenfolioClient();
        public static string ZenfolioResponse { get; private set; }
        /// <summary>
        /// Tree node types enumeration
        /// </summary>
        public enum NodeType
        {
            Root = 0,
            Group,
            Gallery,
            Collection,
            Photo
        }

        public static bool Login(string username, string password)
        {
            try
            {
                var result = Client.Login(username, password);
                return result;
            }
            catch (Exception ex)
            {
                ZenfolioResponse = ex.Message;
                return false;
            }

        }
        #region GetChildren

        public static dynamic[] GetChildren(dynamic parent)
        {
            return new dynamic[]
            {
                //Query RootNode for children
            };
        }

        #endregion // GetChildren

        #region GetPhotos

        public static ObservableCollection<Photo> GetPhotos(PhotoSet photoSet)
        {
            switch (photoSet.Title)
            {
                //Query group for PhotoSets
            }

            return null;
        }

        #endregion // GetPhotos

        #region GetPhotoSets

        public static ObservableCollection<PhotoSet> GetPhotoSets(Group group)
        {
            switch (group.Title)
            {
                //Query group for PhotoSets
            }

            return null;
        }

        #endregion // GetPhotoSets

        #region GetPhotoSetGroups

        public static ObservableCollection<Group> GetPhotoSetGroups(Group group)
        {
            switch (group.Title)
            {
                //query group for other groups
            }

            return null;
        }

        #endregion // GetPhotoSetGroups

        #region GetRootGroup

        public static Group GetRootGroup()
        {
            Group rootGroup = new Group();

            if (false)
            {
                return ReadRecords();
            }

            try
            {
                // Load own profile
                User user = Client.LoadPrivateProfile();

                // Load and wrap Groups hierarchy
                rootGroup = Client.LoadGroupHierarchy(user.LoginName);
                //RootNode = CreateGroupNode(rootGroup);

                //// Fix-up the root node of the Group hierarchy
                //RootNode.ImageIndex = (int)NodeType.Root;
                //RootNode.SelectedImageIndex = RootNode.ImageIndex;
                //RootNode.Title = user.DisplayName;

                //This doesn't adequately save the file, and I'm not really interested in
                //digging into Newtonsoft to create custom converters to make it work, so, bah!
                //SaveRecords(rootGroup);
            }
            catch (Exception ex)
            {

            }
            return rootGroup;
        }

        #endregion // GetRootGroup

        /// <summary>
        /// Creates a TreeNode representing Group element.
        /// </summary>
        /// <param name="element">Group element to wrap.</param>
        /// <returns>Constructed tree node.</returns>
        //private static TreeNode CreateGroupNode(
        //    GroupElement element
        //    )
        //{
        //    TreeNode ret = new TreeNode(element.Title);
        //    ret.Tag = element;

        //    Group group = element as Group;
        //    PhotoSet photoSet = element as PhotoSet;

        //    if (group != null)
        //    {
        //        ret.ImageIndex = (int)NodeType.Group;
        //        foreach (GroupElement child in group.Elements)
        //            ret.Nodes.Add(CreateGroupNode(child));
        //    }
        //    else if (photoSet != null)
        //    {
        //        ret.ImageIndex = photoSet.Type == PhotoSetType.Gallery
        //                         ? (int)NodeType.Gallery
        //                         : (int)NodeType.Collection;

        //        // photoSets contain photos which are not loaded yet; add
        //        // a dummy node for lazy expansion
        //        if (photoSet.PhotoCount > 0)
        //            ret.Nodes.Add(new TreeNode("_Dummy_"));
        //    }

        //    ret.SelectedImageIndex = ret.ImageIndex;
        //    return ret;
        //}

        
        
        ///This doesn't work
        private static bool SaveRecords(Group group)
        {
            string path = "AllRecords.json";
            try
            {
                var result = JsonConvert.SerializeObject(group);
                System.IO.File.WriteAllText(path, result);
            } catch (Exception ex)
            {
                //do something
                return false;
            }
            return true;
        }

        //Or possibly the re-hydrating doesn't work.  Dunno.  Don't really care.
        private static Group ReadRecords(string path = null)
        {
            if (path == null) { path = "AllRecords.json"; }
            var allRecords = System.IO.File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<Group>(allRecords);
            return result;
        }
    }
}