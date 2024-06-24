export function createBlobFromStream(fileArray, contentType) {
    let blob = new Blob([fileArray], { type: contentType });
    return URL.createObjectURL(blob);
};
export function scrollToElementById(id) {
    const element = document.getElementById(id);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth' });
    }
}
