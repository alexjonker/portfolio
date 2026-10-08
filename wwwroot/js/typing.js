const TYPE_DELAY_MS = 20;
const ERASE_DELAY_MS = 10;

const getTypedElement = () => document.getElementById('logo-typing');
let animationTimer;

const typeText = (text) => {
    clearInterval(animationTimer);
    const typedElement = getTypedElement();
    if (!typedElement) return;

    let visibleLength = 0;
    typedElement.textContent = '';
    animationTimer = setInterval(() => {
        typedElement.textContent = text.slice(0, ++visibleLength);
        if (visibleLength >= text.length) clearInterval(animationTimer);
    }, TYPE_DELAY_MS);
};

const eraseText = () => {
    clearInterval(animationTimer);
    const typedElement = getTypedElement();
    if (!typedElement?.textContent) return;

    animationTimer = setInterval(() => {
        typedElement.textContent = typedElement.textContent.slice(0, -1);
        if (!typedElement.textContent) clearInterval(animationTimer);
    }, ERASE_DELAY_MS);
};

const onLinkEnter = (event) => {
    const link = event.target.closest?.('a');
    const linkName = link?.textContent.trim().replace(/\s+/g, '_');
    if (!linkName) return;

    const href = link.getAttribute('href');
    let command;

    if (href === '/') {
        command = ' cd ~/';
    } else if (href?.startsWith('https:')) {
        command = ` xdg-open ~/Desktop/${linkName}.desktop`;
    } else if (href?.startsWith('#')) {
        command = ` ~/Scripts/${linkName}.sh`;
    } else {
        command = ` cd ~/${linkName}`;
    }

    typeText(command);
};

const onLinkLeave = (event) => {
    if (event.target.closest?.('a')) eraseText();
};

document.addEventListener('mouseover', onLinkEnter);
document.addEventListener('focusin', onLinkEnter);
document.addEventListener('mouseout', onLinkLeave);
document.addEventListener('focusout', onLinkLeave);