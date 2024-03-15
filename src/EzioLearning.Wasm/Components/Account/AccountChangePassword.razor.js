export function hideMarginInput() {
    setTimeout(() => {
        window.$(`.mud-input.mud-input-text.mud-input-underline`).css('margin-top', 0);
    }, 10);
}