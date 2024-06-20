window.blobService = {
    GetBlobStream: (streamArray, contentType) => {
    let blob = new Blob([streamArray], { type: contentType });
    return URL.createObjectURL(blob);
    }
}
