export function updateAvatar(avatarUrl) {
    window.$(".user-img img").attr("src", avatarUrl);
    window.$("img.avatar-img").attr("src", avatarUrl);
    //console.log('updated avatar');
    window.$('img.account-avatar').attr("src", avatarUrl);
}
export function updateFullName(fullName) {
    window.$(".account-name").text(fullName);

}