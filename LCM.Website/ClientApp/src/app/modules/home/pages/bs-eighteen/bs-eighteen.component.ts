import { Component } from '@angular/core';
import { IResultDto } from 'src/app/shared/models/dto/response/result-dto';
import { BsEighteenService } from '../../../../core/http/bs-eighteen.service'

@Component({
  selector: 'app-bs-eighteen',
  templateUrl: './bs-eighteen.component.html',
  styleUrls: ['./bs-eighteen.component.scss']
})

export class BsEighteenComponent 
{  
  private _uploadType: string = "";

  constructor(
    private _bsEighteenService: BsEighteenService
  ){  }

  reportUpload(data: IResultDto<string>)
  {
    if(data.message)
    {
      alert(data.message);
      return;
    }
    else 
    {
      if (this._uploadType == "b18") 
      {
        this._bsEighteenService.insertS18(data.content).subscribe(c => {
          alert('大18報表上傳成功.')
        })
      }
      else if (this._uploadType == "s18") 
      {
        this._bsEighteenService.insertS18(data.content).subscribe(c => {
          alert('小18報表上傳成功.')
        })
      }
      else if (this._uploadType == "vendor") 
      {
        alert('pk報表產出成功.')
      }
    }
  }

  setUploadType(type: string)
  {
    this._uploadType = type;
  }

  // test():any
  // {
  //   this._bsEighteenService.test().subscribe(c => {
  //     debugger;
  //     const qq = c;
  //   });
  // }
}
