import { Component, Input } from '@angular/core';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.scss']
})
export class SelectComponent 
{
  @Input() public inputOptionData : DropdownOption[] = [{value:"", text:"無"}];
  selectedValue: string = "";//default selected option value

  //setting Observable
  private emitSecledtedValue = new Subject<string>();
  selectedValue$ = this.emitSecledtedValue.asObservable();

  onSelected(value: string)
  {  
    //this.selectedValue = value;
    this.emitSecledtedValue.next(value);
  }
}

export interface DropdownOption {
  value: string;
  text: string;
}
