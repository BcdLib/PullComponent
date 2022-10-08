import "./PullRefresh.scss"

enum PullStatus {
  Awaiting = "awaiting",// 未开始
  Pulling = "pulling",// 已经下拉，未达到高度
  Loosing = "loosing",// 达到高度，等待松手
  Loading = "loading", // 已经松手，正在刷新
  Completed = "completed", // 刷新完成
}

const pullStatusMap = new Map<PullStatus, number>();
pullStatusMap.set(PullStatus.Awaiting, 0);
pullStatusMap.set(PullStatus.Pulling, 1);
pullStatusMap.set(PullStatus.Loosing, 2);
pullStatusMap.set(PullStatus.Loading, 3);
pullStatusMap.set(PullStatus.Completed, 4);

const pullRefreshCache = new WeakMap<HTMLElement, PullRefresh>();

class PullRefresh {
  static init(selector: HTMLElement | string, dotNetObjRef: any) {
    let ele: HTMLElement;
    if (typeof selector === "string") {
      ele = document.querySelector(selector) as HTMLElement;
    } else {
      ele = selector;
    }

    console.log("init", ele, dotNetObjRef);
    if (!pullRefreshCache.has(ele)) {
      const pullRefresh = new PullRefresh(ele, dotNetObjRef);
      pullRefreshCache.set(ele, pullRefresh);
    }
  }

  static getScrollTop() {
    return document.documentElement.scrollTop || document.body.scrollTop || 0;
  }

  dotNetObjRef: any;
  containerDom: HTMLElement;
  tipDom: HTMLElement;
  wrapperDom: HTMLElement;

  pullStatus: PullStatus = PullStatus.Awaiting;
  moveDistance: number = 0;
  startY: number = 0;

  maxDistance: number = 50;

  constructor(containerDom: HTMLElement, dotNetObjRef: any) {
    this.containerDom = containerDom;
    this.wrapperDom = containerDom.querySelector(
      ".pull-wrapper"
    ) as HTMLElement;
    this.tipDom = containerDom.querySelector(".pull-tip") as HTMLElement;
    this.dotNetObjRef = dotNetObjRef;

    this.bindEvent();
  }

  public bindEvent() {
    const wrapperDom = this.wrapperDom;
    wrapperDom.addEventListener("touchstart", (e) => {
      this.onTouchStart(e);
    });
    wrapperDom.addEventListener("touchmove", (e) => {
      this.onTouchMove(e);
    });
    wrapperDom.addEventListener("touchend", (e) => {
      this.onTouchEnd(e);
    });
    return this;
  }

  private async onTouchStart(e: TouchEvent) {
    console.log("onTouchStart");
    if (
      this.pullStatus === PullStatus.Awaiting ||
      this.pullStatus === PullStatus.Completed
    ) {
      await this.setPullStatus(PullStatus.Pulling);
      // 获取初始y轴位置
      this.startY = e.targetTouches[0].clientY;
      // 触摸开始时，动画时间，移动距离归0
      this.moveDistance = 0;
      console.log("this.tipDom.style");
    }
  }

  private async onTouchMove(e: TouchEvent) {
    // 如果正在加载 则阻止触发
    if (
      this.pullStatus === PullStatus.Pulling ||
      this.pullStatus === PullStatus.Loosing
    ) {
      // 首先计算当前页面是否有滚动条，有滚动条，那么触摸滑动就是页面简单的上下滚动
      let scrollTop =
        document.documentElement.scrollTop || document.body.scrollTop;
      if (scrollTop > 0) return;
      let move = e.targetTouches[0].clientY - this.startY;
      // 判断手指滑动的距离，只有为正数才代表用户下拉了。
      if (move > 0) {
        await this.setDistance(move);
      }
    }
  }

  private async onTouchEnd(_e: TouchEvent) {
    console.log("onTouchEnd this.pullStatus ", this.pullStatus);
    if (this.pullStatus === PullStatus.Loosing) {
      await this.setDistance(this.maxDistance, true);
      setTimeout(async () => {
        await this.setDistance(0);
      }, 1000);
    } else if (
      this.pullStatus == PullStatus.Awaiting ||
      this.pullStatus == PullStatus.Pulling
    ) {
      await this.setDistance(-1, false);
    }
  }

  private wrapperDomStyle = "";

  private async setDistance(moveDist: number, isLoading: boolean = false) {
    const { maxDistance } = this;
    // console.log("setDistance", this.pullStatus, moveDist);
    if (
      isLoading &&
      this.pullStatus != PullStatus.Loading &&
      this.pullStatus != PullStatus.Completed
    ) {
      await this.setPullStatus(PullStatus.Loading);
      await this.setPullStatus(PullStatus.Completed);
    } else if (moveDist <= 0) {
        if(this.pullStatus !== PullStatus.Awaiting){
            console.log("Awaiting");
            this.moveDistance = 0;
            this.wrapperDomStyle = "";
            this.wrapperDom.style.transform = `translate3d(0, 0, 0)`;
            await this.setPullStatus(PullStatus.Awaiting);
        }
    } else {
      let moveDistance = Math.pow(moveDist, 0.8);
      if (moveDistance < maxDistance) {
        if (this.pullStatus != PullStatus.Pulling) {
          await this.setPullStatus(PullStatus.Pulling);
        }
      } else {
        console.log(moveDist);
        moveDistance = maxDistance;
        await this.setPullStatus(PullStatus.Loosing);
      }
      if (this.moveDistance != moveDistance) {
        this.moveDistance = moveDistance;
        // console.log("moveDistance", moveDistance, maxDistance)
        this.wrapperDomStyle = `transform: translate3d(0, ${moveDistance}px, 0)`;
        this.wrapperDom.style.transform = `translate3d(0, ${moveDistance}px, 0)`;
      }
    }
  }

  private async setPullStatus(newPullStatus: PullStatus) {
    if (this.pullStatus != newPullStatus) {
      console.log("changeStatus", newPullStatus);
      //(this.tipDom.querySelector(".pull-" + this.pullStatus)! as HTMLElement).style.display = "none";
      //(this.tipDom.querySelector(".pull-" + newPullStatus)! as HTMLElement).style.display = "flex";
      this.pullStatus = newPullStatus;
      await this.dotNetObjRef.invokeMethodAsync(
        "ChangeStatusAsync",
        pullStatusMap.get(newPullStatus),
        this.wrapperDomStyle
      );
    }
  }
}


export { PullRefresh };