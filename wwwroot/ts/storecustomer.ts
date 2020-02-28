
class storecustomer
{
    private name:string;
    constructor(private str:string) {
        this.name = str;
    }
    public showName() {
        alert(this.name);
    }
}

