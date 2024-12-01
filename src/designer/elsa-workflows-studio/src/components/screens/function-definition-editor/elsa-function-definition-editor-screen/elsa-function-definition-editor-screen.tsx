import { Component, Event, EventEmitter, h, Host, Listen, Method, Prop, State, Watch } from '@stencil/core';
import { injectHistory, RouterHistory } from '@stencil/router';
import {} from '../../../../models';
import { ActivityStats, createElsaClient, eventBus, featuresDataManager, monacoEditorDialogService } from '../../../../services';
import state from '../../../../utils/store';
import DashboardTunnel from '../../../../data/dashboard';
import { downloadFromBlob } from '../../../../utils/download';
import { i18n } from 'i18next';
import { loadTranslations } from '../../../i18n/i18n-loader';
import { resources } from './localizations';
import * as collection from 'lodash/collection';
import { tr } from 'cronstrue/dist/i18n/locales/tr';
import { Monaco, initializeMonacoWorker } from '../../../controls/elsa-monaco/elsa-monaco-utils';
import { GetIntlMessage, IntlMessage } from '../../../i18n/intl-message';
import { DrpMonacoValueChangedArgs } from './drp-monaco-editor';
import axios from 'axios';
import { ConsoleLogger } from '@microsoft/signalr/dist/esm/Utils';
import Swal from 'sweetalert2';

@Component({
  tag: 'elsa-function-definition-editor-screen',
  styleUrl: 'elsa-function-definition-editor-screen.css',
  shadow: false,
})
export class ElsaFunctionDefinitionEditorScreen {
  @Prop({ attribute: 'function-definition-id', reflect: true }) functionDefinitionId: string;
  @Prop({ attribute: 'server-url', reflect: true }) serverUrl: string;
  @Prop({ attribute: 'monaco-lib-path', reflect: true }) monacoLibPath: string;
  @Prop() culture: string;
  private i18next: i18n;
  private mainMonaco: HTMLDrpMonacoEditorElement;
  private sampleInputMonaco: HTMLDrpMonacoEditorElement;
  private outputMonaco: HTMLDrpMonacoEditorElement;
  private functionType: string = 'Function';

  suggestions: any;
  el: HTMLElement;

  @Method()
  async compileFunction(requestData: any) {
    let result: any = null; // Biến tạm lưu trữ kết quả

    try {
      const response = await axios.post(this.serverUrl + '/v1/function-definitions/compile', requestData, {
        headers: {
          'Content-Type': 'application/json', // Định dạng dữ liệu gửi
        },
      });

      result = response.data; // Lưu trữ kết quả
    } catch (error) {
      if (error.response) {
        console.error('Error response:', error.response.data);
        result = error.response.data; // Lưu lỗi từ phản hồi
      } else {
        console.error('Error:', error.message);
        result = { error: error.message }; // Trả về lỗi khác
      }
    }

    return result; // Trả về kết quả, dù thành công hay thất bại
  }
  @Method()
  async formatFunction(requestData: any) {
    let result: any = null; // Biến tạm lưu trữ kết quả

    try {
      const response = await axios.post(this.serverUrl + '/v1/function-definitions/format', requestData, {
        headers: {
          'Content-Type': 'application/json', // Định dạng dữ liệu gửi
        },
      });

      result = response.data; // Lưu trữ kết quả
    } catch (error) {
      if (error.response) {
        console.error('Error response:', error.response.data);
        result = error.response.data; // Lưu lỗi từ phản hồi
      } else {
        console.error('Error:', error.message);
        result = { error: error.message }; // Trả về lỗi khác
      }
    }

    return result; // Trả về kết quả, dù thành công hay thất bại
  }
  @Method()
  async runFunction(requestData: any) {
    let result: any = null; // Biến tạm lưu trữ kết quả

    try {
      const response = await axios.post(this.serverUrl + '/v1/function-definitions/runfunction', requestData, {
        headers: {
          'Content-Type': 'application/json', // Định dạng dữ liệu gửi
        },
      });

      result = response.data; // Lưu trữ kết quả
    } catch (error) {
      if (error.response) {
        console.error('Error response:', error.response.data);
        result = error.response.data; // Lưu lỗi từ phản hồi
      } else {
        console.error('Error:', error.message);
        result = { error: error.message }; // Trả về lỗi khác
      }
    }

    return result; // Trả về kết quả, dù thành công hay thất bại
  }
  @Method()
  async saveFunction(requestData: any) {
    let result: any = null; // Biến tạm lưu trữ kết quả

    try {
      const response = await axios.post(this.serverUrl + '/v1/function-definitions/save', requestData, {
        headers: {
          'Content-Type': 'application/json', // Định dạng dữ liệu gửi
        },
      });

      result = response.data; // Lưu trữ kết quả
    } catch (error) {
      if (error.response) {
        console.error('Error response:', error.response.data);
        result = error.response.data; // Lưu lỗi từ phản hồi
      } else {
        console.error('Error:', error.message);
        result = { error: error.message }; // Trả về lỗi khác
      }
    }

    return result; // Trả về kết quả, dù thành công hay thất bại
  }
  @Method()
  async getDetailFunction() {
    let result: any = null; // Biến tạm lưu trữ kết quả

    try {
      const response = await axios.get(this.serverUrl + '/v1/function-definitions/' + this.functionDefinitionId);

      result = response.data; // Lưu trữ kết quả
    } catch (error) {
      if (error.response) {
        console.error('Error response:', error.response.data);
        result = error.response.data; // Lưu lỗi từ phản hồi
      } else {
        console.error('Error:', error.message);
        result = { error: error.message }; // Trả về lỗi khác
      }
    }

    return result; // Trả về kết quả, dù thành công hay thất bại
  }
  @Method()
  async getFunctionTemplate() {
    let result: any = null; // Biến tạm lưu trữ kết quả

    try {
      const response = await axios.get(this.serverUrl + '/v1/function-definitions/GetTemplate/' + this.functionType);

      result = response.data.data; // Lưu trữ kết quả
    } catch (error) {
      if (error.response) {
        console.error('Error response:', error.response.data);
        result = error.response.data; // Lưu lỗi từ phản hồi
      } else {
        console.error('Error:', error.message);
        result = { error: error.message }; // Trả về lỗi khác
      }
    }

    return result; // Trả về kết quả, dù thành công hay thất bại
  }
  @Method()
  async OnCompileFunctionClick(e) {
    const requestData: any = {
      source: this.mainMonaco.value,
      functionId: '1',
      sampleInput: '',
      catalog: '',
      functionType: this.functionType,
    };
    let compileResult = await this.compileFunction(requestData);
    if (compileResult.isSuccess) {
      Swal.fire({
        title: compileResult.isSuccess ? 'Successfully' : 'Failed',
        text: compileResult.message,
        icon: compileResult.isSuccess ? 'success' : 'error',
        confirmButtonText: 'OK',
        confirmButtonColor: '#3b82f6',
      });
      this.outputMonaco.setValue('');
    } else {
      this.outputMonaco.setValue(compileResult.message);
    }
  }
  @Method()
  async OnFormatFunctionClick(e) {
    const requestData: any = {
      source: this.mainMonaco.value,
      functionId: '1',
      sampleInput: '',
      catalog: '',
      functionType: this.functionType,
    };
    let formatResult = await this.formatFunction(requestData);
    if (formatResult.isSuccess) {
      this.mainMonaco.setValue(formatResult.data);
    } else {
      this.outputMonaco.setValue(formatResult.message);
    }
  }
  @Method()
  async OnRunFunctionClick(e) {
    const requestData: any = {
      source: this.mainMonaco.value,
      functionId: '1',
      sampleInput: this.sampleInputMonaco.value,
      catalog: '',
      functionType: this.functionType,
    };
    let runResult = await this.runFunction(requestData);
    if (runResult.isSuccess) {
      this.handleRunResult(runResult);
    } else {
      this.outputMonaco.setValue(runResult.message);
    }
  }
  handleRunResult(runResult: any) {
    try {
      const beautifiedJson = JSON.stringify(runResult.data, null, 2); // Beautify JSON
      this.outputMonaco.setValue(beautifiedJson); // Set beautified JSON value
    } catch (error) {
      console.log(error);
      // If it's not valid JSON, handle error or set raw string
      console.error('Invalid JSON:', error);
      this.outputMonaco.setValue(runResult.data.toString());
    }
  }
  @Method()
  async OnSaveFunctionClick(e) {
    const requestData: any = {
      source: this.mainMonaco.value,
      functionId: this.functionDefinitionId,
      sampleInput: this.sampleInputMonaco.value,
      catalog: this.functionType,
      functionType: this.functionType,
    };
    let compileResult = await this.saveFunction(requestData);
    if (compileResult.isSuccess) {
      Swal.fire({
        title: compileResult.isSuccess ? 'Successfully' : 'Failed',
        text: compileResult.message,
        icon: compileResult.isSuccess ? 'success' : 'error',
        confirmButtonText: 'OK',
        confirmButtonColor: '#3b82f6',
      });
      this.outputMonaco.setValue('');
    } else {
      this.outputMonaco.setValue(compileResult.message);
    }
  }
  @Method()
  async LoadSourceCodeIntoEditor() {
    let detail = await this.getDetailFunction();
    this.mainMonaco.setValue(detail.source);
    this.sampleInputMonaco.setValue(detail.sampleInput);
  }
  @Method()
  async onMainMonacoValueChanged(e: DrpMonacoValueChangedArgs) {
    this.mainMonaco.value = e.value;
  }
  @Method()
  async onSampleInputMonacoValueChanged(e: DrpMonacoValueChangedArgs) {
    this.sampleInputMonaco.value = e.value;
  }
  @Method()
  async onOutputMonacoValueChanged(e: DrpMonacoValueChangedArgs) {
    this.outputMonaco.value = e.value;
  }

  async componentWillLoad() {
    this.i18next = await loadTranslations(this.culture, resources);
    await this.monacoLibPathChangedHandler(this.monacoLibPath);
  }
  async componentDidLoad() {
    if (this.functionDefinitionId) {
      let loadingTexts = [
        `/***
 *      ____           ____           ____    
 *     |  _"\\       U |  _"\\ u      U|  _"\\ u 
 *    /| | | |       \\| |_) |/      \\| |_) |/ 
 *    U| |_| |\\       |  _ <         |  __/   
 *     |____/ u       |_| \\_\\        |_|      
 *      |||_          //   \\\\_       ||>>_    
 *     (__)_)        (__)  (__)     (__)__)   
 */`,
        `/***
 *                                                                           
 *                                                                           
 *    DDDDDDDDDDDDD             RRRRRRRRRRRRRRRRR        PPPPPPPPPPPPPPPPP   
 *    D::::::::::::DDD          R::::::::::::::::R       P::::::::::::::::P  
 *    D:::::::::::::::DD        R::::::RRRRRR:::::R      P::::::PPPPPP:::::P 
 *    DDD:::::DDDDD:::::D       RR:::::R     R:::::R     PP:::::P     P:::::P
 *      D:::::D    D:::::D        R::::R     R:::::R       P::::P     P:::::P
 *      D:::::D     D:::::D       R::::R     R:::::R       P::::P     P:::::P
 *      D:::::D     D:::::D       R::::RRRRRR:::::R        P::::PPPPPP:::::P 
 *      D:::::D     D:::::D       R:::::::::::::RR         P:::::::::::::PP  
 *      D:::::D     D:::::D       R::::RRRRRR:::::R        P::::PPPPPPPPP    
 *      D:::::D     D:::::D       R::::R     R:::::R       P::::P            
 *      D:::::D     D:::::D       R::::R     R:::::R       P::::P            
 *      D:::::D    D:::::D        R::::R     R:::::R       P::::P            
 *    DDD:::::DDDDD:::::D       RR:::::R     R:::::R     PP::::::PP          
 *    D:::::::::::::::DD        R::::::R     R:::::R     P::::::::P          
 *    D::::::::::::DDD          R::::::R     R:::::R     P::::::::P          
 *    DDDDDDDDDDDDD             RRRRRRRR     RRRRRRR     PPPPPPPPPP          
 *                                                                                                                                              
 *                                                                           
 */`,
        `/***
 *     ______       _______        _______   
 *    |_   _ \`.    |_   __ \\      |_   __ \\  
 *      | | \`. \\     | |__) |       | |__) | 
 *      | |  | |     |  __ /        |  ___/  
 *     _| |_.' /    _| |  \\ \\_     _| |_     
 *    |______.'    |____| |___|   |_____|    
 *                                           
 */`,
        `
/***
 *    ██████╗     ██████╗     ██████╗ 
 *    ██╔══██╗    ██╔══██╗    ██╔══██╗
 *    ██║  ██║    ██████╔╝    ██████╔╝
 *    ██║  ██║    ██╔══██╗    ██╔═══╝ 
 *    ██████╔╝    ██║  ██║    ██║     
 *    ╚═════╝     ╚═╝  ╚═╝    ╚═╝     
 *                                    
 */
`,
      ];
      const randomIndex = Math.floor(Math.random() * loadingTexts.length);
      this.mainMonaco.setValue(loadingTexts[randomIndex]);

      this.sampleInputMonaco.setValue('From HuyNT and DRP with love <3');

      this.LoadSourceCodeIntoEditor();
    }else{
      this.mainMonaco.setValue(await this.getFunctionTemplate());
    }
  }
  @Watch('monacoLibPath')
  async monacoLibPathChangedHandler(newValue: string) {
    state.monacoLibPath = newValue;
  }

  @Method()
  async handleFunctionTypeSelectChange(event) {
    const selectElement = event.target as HTMLSelectElement;
    this.functionType = selectElement.value;
    this.mainMonaco.setValue(await this.getFunctionTemplate());
  }

  render() {
    const IntlMessage = GetIntlMessage(this.i18next);
    return (
      <div>
        <div class="elsa-h-16 elsa-border-b elsa-border-gray-600 elsa-px-4 elsa-py-4 sm:elsa-flex sm:elsa-items-center sm:elsa-justify-between sm:elsa-px-6 lg:elsa-px-8 elsa-bg-gray-700">
          <div class="elsa-flex elsa-items-center elsa-space-x-4 elsa-bg-gray-700">
            <h1 class="elsa-text-lg elsa-font-medium elsa-leading-6 elsa-text-gray-300 sm:elsa-truncate">
              <IntlMessage label="Title" />
            </h1>
          </div>
          <div class="elsa-hidden sm:elsa-flex elsa-space-x-6">
            <select
              class="elsa-bg-gray-700 elsa-text-gray-300 focus:elsa-ring-blue-500 elsa-w-full elsa-shadow-sm sm:elsa-text-sm elsa-border-gray-600 elsa-rounded-md"
              name="functionType"
              id="functionType"
              onChange={e => this.handleFunctionTypeSelectChange(e)}
            >
              <option selected value="Function" class="elsa-bg-gray-700">
                Function
              </option>
              <option value="SharedUtility">Shared Utility</option>
            </select>
          </div>
          <div class="elsa-flex elsa-items-center elsa-space-x-4 elsa-bg-gray-700">
            <button
              onClick={e => this.OnCompileFunctionClick(e)}
              id="complie"
              type="button"
              class="elsa-bg-orange elsa-text-gray-200 elsa-px-4 elsa-py-2 elsa-rounded hover:elsa-bg-orange hover:elsa-shadow-lg"
            >
              Compile Function
            </button>
            <button
              onClick={e => this.OnFormatFunctionClick(e)}
              id=""
              type="button"
              class="elsa-bg-pink-500 elsa-text-gray-200 elsa-px-4 elsa-py-2 elsa-rounded hover:elsa-bg-pink-700 hover:elsa-shadow-lg"
            >
              Format Code
            </button>
            <button
              onClick={e => this.OnRunFunctionClick(e)}
              id=""
              type="button"
              class="elsa-bg-green-500 elsa-text-gray-200 elsa-px-4 elsa-py-2 elsa-rounded hover:elsa-bg-green-700 hover:elsa-shadow-lg"
            >
              Run Function
            </button>
            <button
              onClick={e => this.OnSaveFunctionClick(e)}
              id="save"
              type="button"
              class="elsa-bg-blue-500 elsa-text-gray-200 elsa-px-4 elsa-py-2 elsa-rounded hover:elsa-bg-blue-700 hover:elsa-shadow-lg"
            >
              Save & Publish
            </button>
          </div>
        </div>
        <div class="elsa-flex elsa-h-full">
          <div class="elsa-w-9/12 col-1 elsa-flex-1">
            <drp-monaco-editor
              server-base-url={this.serverUrl}
              ref={el => (this.mainMonaco = el)}
              onValueChanged={e => this.onMainMonacoValueChanged(e.detail)}
              id="mainMonaco"
              value=""
              theme="vs-dark"
              renderLineHighlight="all"
              editor-height="100%"
              single-line={false}
              padding="elsa-p-1"
              language="csharp"
            />
          </div>

          {/* <!-- Cột bên phải --> */}
          <div class="elsa-w-3/12 elsa-flex elsa-flex-col h-full-minus-header">
            {/* <!-- Phần trên --> */}
            <div class="elsa-h-1/3 elsa-p-1 elsa-flex elsa-flex-col">
              <span class="elsa-text-gray-300">Sample Input</span>
              <div class="elsa-flex-1">
                <drp-monaco-editor
                  onValueChanged={e => this.onSampleInputMonacoValueChanged(e.detail)}
                  ref={el => (this.sampleInputMonaco = el)}
                  value=""
                  theme="vs-dark"
                  renderLineHighlight="none"
                  editor-height="100%"
                  single-line={false}
                  padding="elsa-p-0"
                  language="json"
                  lineNumbers="off"
                  server-base-url={this.serverUrl}
                />
              </div>
            </div>

            {/* <!-- Phần dưới --> */}
            <div class="elsa-h-2/3 elsa-p-1 elsa-flex elsa-flex-col">
              <span class="elsa-text-gray-300">Output</span>
              <div class="elsa-flex-1">
                <drp-monaco-editor
                  id="outputMonaco"
                  server-base-url={this.serverUrl}
                  onValueChanged={e => this.onOutputMonacoValueChanged(e.detail)}
                  ref={el => (this.outputMonaco = el)}
                  value=""
                  theme="vs-dark"
                  renderLineHighlight="none"
                  editor-height="100%"
                  single-line={false}
                  padding="elsa-p-0"
                  language="json"
                  lineNumbers="off"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

injectHistory(ElsaFunctionDefinitionEditorScreen);
DashboardTunnel.injectProps(ElsaFunctionDefinitionEditorScreen, ['serverUrl', 'culture', 'monacoLibPath', 'basePath', 'serverFeatures']);
