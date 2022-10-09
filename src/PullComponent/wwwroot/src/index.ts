import "./index.scss";

document.addEventListener('scroll', async (e) => {
    await (window as any).DotNet.invokeMethodAsync("BcdLib.PullComponent", "Scroll", {
        distToBottom: Document.getScrollDistToBottom()
    });
});

export class Document {
    static getScrollTop() {
        const scrollTop =
            document.documentElement.scrollTop || document.body.scrollTop || 0;
        return Math.round(scrollTop);
    }

    static getScrollDistToBottom() {
        const dist =
            document.documentElement.scrollHeight -
            document.documentElement.scrollTop -
            document.documentElement.clientHeight;
        return Math.round(dist);
    }
}
