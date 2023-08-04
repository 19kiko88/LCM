import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.scss']
})
export class SelectComponent {
  public selectedValue: string | undefined

  @Input() public inputDisplayText: string = "";
  @Input() public inputOptionData : DropdownOption[] = [{value:"0", text:"無"}];
  @Output() private outputSelectedValue: EventEmitter<string>  = new EventEmitter();//0:開始上傳 1:上傳完成 -1:上傳失敗

  onSelected(value: string)
  {
    this.selectedValue = value;
    this.outputSelectedValue?.emit(value);
  }
}

export interface DropdownOption {
  value: string;
  text: string;
}
