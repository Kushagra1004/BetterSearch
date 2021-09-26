allFileList = [] <FileSystemInfo>

node {
    name : C:/
    fileList : [file1.txt:22jun:size,...]
    directoryList : [dir1, dir2]
    lastmodified: 23 jun
}

# File changes : add, remove, rename, content changed
filechange (fileSystemInfo, operation){
    # renamed, size changed -- updates
    split path with /
    traverse to position
    case operation:
        add -> fileList.push
        rename -> fileList[elem].rename
        remove -> fileList.remove(elem)
        contentChanged -> fileList.[elem].changesize
}


# Folder changes : add, remove, rename
folderChanges(fileSystemInfo, operation){
    split path with /
    traverse to position
    case operation:
        add -> only add node
        remove -> delete node
        rename -> node.name = rename
}

Search (text,node,currentPath){
    currentPath : currentpath + "/ nodename";
    if currentPath.contains(text) then append to uidatasource
    fileList.foreach(x => 
        currentFilepath = currentpath + x.name
        currentFilePath.contains(text) then append to uidatasource);
    folderList.foreach(x=> search(text,x,currentPath))
}





# end of tree funxtions ------------------

allFileList.foreach (x => if x.type is File then fileChange(x,add)
                        else folderChange(x,add));







