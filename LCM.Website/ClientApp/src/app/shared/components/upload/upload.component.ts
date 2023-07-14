import { HttpEventType } from '@angular/common/http';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { BsEighteenService } from 'src/app/core/http/bs-eighteen.service';
import { IResultDto } from '../../models/dto/response/result-dto';
import {SweetAlertService} from '../../../core/service/sweet-alert.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})

export class UploadComponent 
{
  @Input() public inputUploadType: string = "";
  @Output() private outputUploadInfo: EventEmitter<IResultDto<string>>  = new EventEmitter();
  @Output() private startLoading: EventEmitter<boolean>  = new EventEmitter();
  @ViewChild('myFileInput') myFileInput: any;
  
  isLoading: boolean = true;
  selectedFiles?: FileList;
  currentFile?: File;
  progress = 0;
  message = '';
  fileInfos?: Observable<any>;
  fullFilePath: string = '';
  fileName: string = '';


  constructor(
    private _bsEighteenService: BsEighteenService,
    private swl: SweetAlertService
  ) { }

  selectFile(event: any): void
  {
    this.fileName = '';
    this.message = '';
    this.progress = 0;
    this.selectedFiles = undefined;

    if (event.target.files.length > 0)
    {
      if (event.target.files[0].type != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
      {
        this.swl.showSwal("", `附檔名必須為.xlsx`, "error");
        this.selectedFiles = undefined;
        this.myFileInput.nativeElement.value = '';
        return
      }

      if (event.target.files[0].size > 100000000)
      {
        this.swl.showSwal("", `檔案大小不能超過100MB.`, "error");
        this.selectedFiles = undefined;
        this.myFileInput.nativeElement.value = '';
        return
      }

      this.selectedFiles = event.target.files;      
    }
  }

  upload()
  {
    this.startLoading?.emit(true);
    console.log(`${this.inputUploadType} upload start.`);
    this.progress = 0;

    if (this.selectedFiles) 
    {
      const file: File | null = this.selectedFiles.item(0);

      // if (file)
      // {
      //   this.currentFile = file;
      //   this._bsEighteenService.upload(this.inputUploadType, this.currentFile)
      //     .subscribe({
      //       next: async (event) =>
      //       {
      //         if (event.type === HttpEventType.UploadProgress)
      //         {
      //           this.progress = Math.round(100 * event.loaded / event.total);
      //         }
      //         else if (event.type === HttpEventType.Response)
      //         {            
      //           if (event.body)
      //           {
      //             //res = event.body;
      //             console.log(`${this.inputUploadType} upload end.`);
      //             this.selectedFiles = undefined;
      //             this.myFileInput.nativeElement.value = '';
      //             await this.outputUploadInfo?.emit(event.body);
      //           }
      //         }
      //       },
      //       complete: () => {

      //       },
      //       error: () => {
      //         this.startLoading?.emit(false);
      //       }
      //     }

      //   )
      // }

      //this.selectedFiles = undefined;
    }
  } 
}
