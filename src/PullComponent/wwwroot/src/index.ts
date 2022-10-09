import "./test.scss"
import "./index.scss"


export class Document {
    static getScrollTop() {
        const scrollTop = document.documentElement.scrollTop || document.body.scrollTop || 0;
        return Math.round(scrollTop);
    }
}