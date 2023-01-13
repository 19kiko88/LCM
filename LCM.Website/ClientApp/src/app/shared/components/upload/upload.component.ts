import { HttpEventType } from '@angular/common/http';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { BsEighteenService } from 'src/app/core/http/bs-eighteen.service';
import { IResultDto } from '../../models/dto/response/result-dto';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})

export class UploadComponent 
{
  @Input() public inputUploadType: string = "";
  @Output() private outputUploadInfo: EventEmitter<IResultDto<string>>  = new EventEmitter();
  @Output() private outputUploadType: EventEmitter<string>  = new EventEmitter();

  selectedFiles?: FileList;
  currentFile?: File;
  progress = 0;
  message = '';
  fileInfos?: Observable<any>;
  fullFilePath: string = '';
  fileName: string = '';
  isLoading: boolean = false;


  constructor(
    private _bsEighteenService: BsEighteenService
  ) { }

  selectFile(event: any): void
  {
    this.fileName = '';
    this.message = '';
    this.progress = 0;

    if (event.target.files[0].type != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
    {
      //this._swlService.showSwal("", `附檔名必須為.xlsx`, "error");
      return
    }

    this.selectedFiles = event.target.files;
  }

  upload(): void
  {
    this.progress = 0;

    if (this.selectedFiles) {
      const file: File | null = this.selectedFiles.item(0);

      if (file)
      {
        this.currentFile = file;

        this._bsEighteenService.upload(this.inputUploadType, this.currentFile)
          .subscribe({
            next: (event) =>
            {
              if (event.type === HttpEventType.UploadProgress)
              {
                this.progress = Math.round(100 * event.loaded / event.total);
              }
              else if (event.type === HttpEventType.Response)
              {               
                let res: IResultDto<string>;

                if (event.body)
                {
                  debugger;
                  //res = event.body;
                  this.outputUploadType?.emit(this.inputUploadType);
                  this.outputUploadInfo?.emit(event.body);


                  //let qq: IResultDto<string> = JSON.parse(JSON.stringify(event.body));
                  //this.fullFilePath = JSON.parse(JSON.stringify(event.body))["content"];
                  //this.fileName = JSON.parse(JSON.stringify(event.body))["fileName"];
                }

                //this.message = `[${this.fileName}] 上傳成功.`;

                // let res :IResultDto<string> = {
                //   content : this.fullFilePath,
                //   success: boolean;
                //   message: string;
                //   content: T;
                //   exception?: object;
                //   innerResults?: IResultDto<T>[];
                // };
                
                //this.outputUploadInfo?.emit(res);

                //this.patchDeleteForm.controls["txt_path"].setValue('qqq');
                //this.onUploadFinished.emit(event.body);
              }
            }
          }

        //this._reportService.upload(this.currentFile).subscribe({
        //  next: (event: any) =>
        //  {
        //    debugger;
        //    if (event.type === HttpEventType.UploadProgress)
        //    {
        //      this.progress = Math.round(100 * event.loaded / event.total);
        //    }
        //    else if (event instanceof HttpResponse)
        //    {
        //      this.message = event.body.message;
        //      this.fileInfos = this._reportService.getFiles();
        //    }
        //  },
        //  error: (err: any) => {
        //    console.log(err);
        //    this.progress = 0;

        //    if (err.error && err.error.message) {
        //      this.message = err.error.message;
        //    } else {
        //      this.message = 'Could not upload the file!';
        //    }

        //    this.currentFile = undefined;
        //  }
        //});
      )}

      this.selectedFiles = undefined;
    }
  } 
}
