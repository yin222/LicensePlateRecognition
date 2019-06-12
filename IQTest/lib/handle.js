var vm = new Vue({
    el:'#mainBox',
    data:{
        datanumber:'test',
        copyData:'',
        nums : ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0", 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            ],
        number:["1", "2", "3", "4", "5", "6", "7", "8", "9", "0"],
        isExpression:true,
        opt:['+','-','*','/'],
        pageIndex:1,
        result:0,
        datalist:['test1','test2','test3','test4']
    },
    methods: {
        tryAgain(){
            this.pageIndex = 1;
            this.result = 0;
            this.getDataNumber();
            this.getDataList();
            this.run();
        },
        seeAnswer(){
            if(this.pageIndex < 10){
                alert('完成所有测试再查看吧');
                return;
            }
            if(this.result >=0 && this.result <= 2){
                alert('要不再试试?');
            }else if(this.result >=3 && this.result <= 6){
                alert('得注意点了')
            }else{
                alert('记忆力很棒啊')
            }
            
        },
        answerClick(isRight){
            if(this.pageIndex >9)
            {
                return;
            }else{
                if(this.isExpression){
                    if(isRight == eval(this.copyData)){
                        ++this.result;
                    }
                }else{
                    if(isRight == this.copyData){
                        ++this.result;
                    }
                }
                this.pageIndex++;
                this.getDataNumber();
                this.getDataList();
                this.run();
            }
        },
        getDataList(){
            this.datalist = ['','','',''];
            var index = parseInt(Math.random()*3+1);
            if(this.isExpression){
                this.datalist.splice(index,1,eval(this.datanumber));
                this.datalist.some((item,i)=>{
                    if(item == ''){
                        this.datalist[i] = eval(this.getExpressionResult());
                    }
                })
            }else{
                this.datalist.splice(index,1,this.datanumber);
                this.datalist.some((item,i)=>{
                    if(item == ''){
                        this.datalist[i] = this.datanumber[0]+this.getSelectNumber(this.pageIndex);
                    }
                })
            }
        },
        getSelectNumber(num){
            var data = '';
            for(var i = 0;i<num;i++){
                var index = parseInt(Math.random()*this.nums.length);
                data += this.nums[index];
            }
            return data;
         },
         getExpressionResult(){
            var numberIndex = parseInt(Math.random()*this.number.length);
            var number2Index = parseInt(Math.random()*this.number.length);
            var optIndex = parseInt(Math.random()*this.opt.length);
            return this.number[numberIndex].toString()+this.opt[optIndex].toString()+this.number[number2Index].toString();
         },
        getDataNumber(){
            var index = parseInt(Math.random()*2+1);

            if(index == 1){
                this.isExpression = true;
                this.copyData = this.datanumber =this.getExpressionResult();
            }else{
                this.isExpression = false;
                this.copyData = this.datanumber = this.getSelectNumber(this.pageIndex+1);
            }
        },
        run(){
            setTimeout(()=>{
                this.datanumber = '';
            },500)
        }
    },
    mounted(){
        this.getDataNumber();
        this.getDataList();
        this.run();
    }
   /*  components:{
        problemgenerate:{
            template:'#problem_com',
            data(){
                return{};
            },
            props:['pardata'],
            methods:{
                run(){
                    setTimeout(()=>{
                        console.log(this.pardata);

                    },500);
                    console.log(this.pardata);
                    this.pardata = '1231';  
                }
            }
        },
        tippro:{
            template:'#tip_com',
            data(){
                return {pageIndex:1};
            }
        },
        selectpro:{
            template:'#select_com'
        }
    }, */

})