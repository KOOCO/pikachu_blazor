window.convertBlobUrlToByteArray = async (blobUrl) => {
    const response = await fetch(blobUrl);
    const arrayBuffer = await response.arrayBuffer();
    const byteArray = new Uint8Array(arrayBuffer);
    const base64 = btoa(String.fromCharCode(...byteArray));
    return base64;
};