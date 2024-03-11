export function updateAvatar(avatarUrl) {
    window.$(".user-img img").attr("src", avatarUrl);
    window.$("img.avatar-img").attr("src", avatarUrl);
    console.log('updated avatar');
}